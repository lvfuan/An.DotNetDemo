using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net.Sockets;
using System.Text;
using FS.Configs;
using FS.Extends;
using FS.Redis.Infrastructure.Pipeline;
using FS.Utils.Common;

namespace FS.Redis.Internal
{
    /// <summary>
    ///     Redis客户端
    /// </summary>
    internal class RedisClient : IDisposable
    {
        /// <summary>
        ///     Redis客户端
        /// </summary>
        /// <param name="redisConnection">上下文数据库连接信息</param>
        public RedisClient(Connection redisConnection)
        {
            _redisConnection = redisConnection;
        }

        /// <summary>
        ///     上下文数据库连接信息
        /// </summary>
        private readonly Connection _redisConnection;

        /// <summary>
        ///     最后一次连接的时间
        /// </summary>
        private long _lastConnectedAtTimestamp;

        /// <summary>
        ///     客户端连接
        /// </summary>
        private Socket _socket;

        /// <summary>
        ///     Socket缓冲流
        /// </summary>
        private BufferedStream _socketStream;

        /// <summary>
        ///     库
        /// </summary>
        private long _db;

        /// <summary>
        ///     是否出错了
        /// </summary>
        private bool _isError;

        /// <summary>
        ///     最后一次发送的命令内容
        /// </summary>
        private string _lastCommand;

        /// <summary>
        ///     缓冲区管理
        /// </summary>
        private readonly BufferManger _bufferManger = new BufferManger();

        private IRedisPipelineShared _pipeline;

        private IRedisPipelineShared Pipeline
        {
            get { return _pipeline; }
            set
            {
                if (value != null) { ConnectedAndReset(); }
                _pipeline = value;
            }
        }

        #region 发送命令管理

        /// <summary>
        ///     发送命令到服务端
        /// </summary>
        /// <param name="sendCmds">要发送的命令（FS.Redis.Internal.Commands）、内容</param>
        private bool SendCommand(params byte[][] sendCmds)
        {
            // 判断是否连接到服务端
            if (!ConnectedAndReset())
            {
                Check.IsTure(true, $"连接Redis服务器失败：命令：{sendCmds[0].ToStringByUtf8()}");
                return false;
            }

            // 将命令、内容根据Redis协议进行组装并写入缓冲区
            _bufferManger.WriteAll(sendCmds);

            //pipeline will handle flush, if pipelining is turned on
            if (Pipeline == null) { return _bufferManger.Send(_socket); }
            return true;
        }
        /// <summary>
        ///     发送消息（返回成功结果）
        /// </summary>
        /// <param name="sendCmds">要发送的命令（FS.Redis.Internal.Commands）、内容</param>
        public void SendExpectOk(params byte[][] sendCmds)
        {
            SendCommand(sendCmds);
            if (Pipeline != null) { Pipeline.CompleteVoidQueuedCommand(ReadSuccess); return; }
            ExpectOk();
        }

        /// <summary>
        ///     发送消息（无需返回数据）
        /// </summary>
        /// <param name="sendCmds">要发送的命令（FS.Redis.Internal.Commands）、内容</param>
        public void SendExpectSuccess(params byte[][] sendCmds)
        {
            SendCommand(sendCmds);
            if (Pipeline != null) { Pipeline.CompleteVoidQueuedCommand(ReadSuccess); return; }
            ReadSuccess();
        }

        /// <summary>
        ///     发送消息（返回列表数据）
        /// </summary>
        /// <param name="sendCmds">要发送的命令（FS.Redis.Internal.Commands）、内容</param>
        public byte[][] SendExpectMultiData(params byte[][] sendCmds)
        {
            SendCommand(sendCmds);

            if (Pipeline != null) { Pipeline.CompleteMultiBytesQueuedCommand(ReadMultiByte); return new byte[0][]; }
            return ReadMultiByte();
        }

        /// <summary>
        ///     发送消息（返回字符串数据）
        /// </summary>
        /// <param name="sendCmds">要发送的命令（FS.Redis.Internal.Commands）、内容</param>
        public string SendExpectString(params byte[][] sendCmds)
        {
            SendCommand(sendCmds);
            if (Pipeline != null) { Pipeline.CompleteMultiBytesQueuedCommand(ReadMultiByte); return null; }
            return ReadString();
        }

        /// <summary>
        ///     发送消息（返回long数据）
        /// </summary>
        /// <param name="sendCmds">要发送的命令（FS.Redis.Internal.Commands）、内容</param>
        public long SendExpectLong(params byte[][] sendCmds)
        {
            SendCommand(sendCmds);
            if (Pipeline != null) { Pipeline.CompleteLongQueuedCommand(ReadLong); return default(long); }
            return ReadLong();
        }
        /// <summary>
        ///     发送消息（返回int数据）
        /// </summary>
        /// <param name="sendCmds">要发送的命令（FS.Redis.Internal.Commands）、内容</param>
        public int SendExpectInt(params byte[][] sendCmds)
        {
            SendCommand(sendCmds);
            if (Pipeline != null) { Pipeline.CompleteIntQueuedCommand(ReadInt); return default(int); }
            return ReadInt();
        }

        /// <summary>
        ///     发送消息（返回字节数据）
        /// </summary>
        /// <param name="sendCmds">要发送的命令（FS.Redis.Internal.Commands）、内容</param>
        public byte[] SendExpectData(params byte[][] sendCmds)
        {
            SendCommand(sendCmds);
            if (Pipeline != null) { Pipeline.CompleteBytesQueuedCommand(ReadByte); return null; }
            return ReadByte();
        }

        /// <summary>
        ///     发送消息（返回double数据）
        /// </summary>
        /// <param name="sendCmds">要发送的命令（FS.Redis.Internal.Commands）、内容</param>
        public double SendExpectDouble(params byte[][] sendCmds)
        {
            SendCommand(sendCmds);
            if (Pipeline != null) { Pipeline.CompleteDoubleQueuedCommand(ReadDouble); return Double.NaN; }
            return ReadDouble();
        }

        /// <summary>
        ///     发送消息（返回Code数据）
        /// </summary>
        /// <param name="sendCmds">要发送的命令（FS.Redis.Internal.Commands）、内容</param>
        public string SendExpectCode(params byte[][] sendCmds)
        {
            SendCommand(sendCmds);
            if (Pipeline != null) { Pipeline.CompleteStringQueuedCommand(ExpectCode); return null; }
            return ExpectCode();
        }

        /// <summary>
        ///     发送消息（返回字符串数据）
        /// </summary>
        /// <param name="sendCmds">要发送的命令（FS.Redis.Internal.Commands）、内容</param>
        public object[] SendExpectDeeplyNestedMultiData(params byte[][] sendCmds)
        {
            SendCommand(sendCmds);
            if (Pipeline != null) { throw new NotSupportedException("Pipeline is not supported."); }
            return ReadDeeplyNestedMultiData();
        }

        #endregion

        #region 接收命令管理

        /// <summary>
        ///     判断接收结果是否成功
        /// </summary>
        private void ReadSuccess()
        {
            var resultPrefx = ReadPrefx();
            var s = ReadString();

            Check.IsTure(resultPrefx == '-', s.StartsWith("ERR") && s.Length >= 4 ? s.Substring(4) : s);
        }

        /// <summary>
        ///     读取结果，返回字符串
        /// </summary>
        private string ReadString()
        {
            var sb = new StringBuilder();

            int c;
            while ((c = _socketStream.ReadByte()) != -1)
            {
                if (c == '\r') { continue; }
                if (c == '\n') { break; }
                sb.Append((char)c);
            }
            return sb.ToString();
        }

        /// <summary>
        ///     读取结果，返回字节数组
        /// </summary>
        public byte[][] ReadMultiByte()
        {
            var resultPrefx = ReadPrefx();
            var msg = ReadString();

            switch (resultPrefx)
            {
                // Some commands like BRPOPLPUSH may return Bulk Reply instead of Multi-bulk
                case '$':
                    var t = new byte[2][];
                    t[1] = ParseSingleLine(string.Concat(char.ToString((char)resultPrefx), msg));
                    return t;

                case '-':
                    throw new Exception(msg.StartsWith("ERR") ? msg.Substring(4) : msg);
                case '*':
                    int count;
                    if (int.TryParse(msg, out count))
                    {
                        //redis is in an invalid state
                        if (count == -1) { return new byte[0][]; }
                        var result = new byte[count][];
                        for (var i = 0; i < count; i++) { result[i] = ReadByte(); }
                        return result;
                    }
                    break;
            }

            throw new Exception("未知的消息格式: " + resultPrefx + msg);
        }

        /// <summary>
        ///     读取结果，返回字节
        /// </summary>
        private byte[] ReadByte()
        {
            var r = ReadString();
            return ParseSingleLine(r);
        }
        /// <summary>
        ///     读取结果，返回字节数组
        /// </summary>
        private string ExpectCode()
        {
            var resultPrefx = ReadPrefx();
            var msg = ReadString();

            if (resultPrefx == '-') { throw new Exception(msg.StartsWith("ERR") ? msg.Substring(4) : msg); }

            return msg;
        }
        /// <summary>
        ///     读取结果，要求与OK一致
        /// </summary>
        private void ExpectOk()
        {
            ExpectWord("OK");
        }
        /// <summary>
        ///     读取结果，要求与QUEUED一致
        /// </summary>
        private void ExpectQueued()
        {
            ExpectWord("QUEUED");
        }
        /// <summary>
        /// 读取结果，要求与word一致
        /// </summary>
        /// <param name="word"></param>
        private void ExpectWord(string word)
        {
            var resultPrefx = ReadPrefx();
            var msg = ReadString();

            if (resultPrefx == '-') { throw new Exception(msg.StartsWith("ERR") ? msg.Substring(4) : msg); }

            if (msg != word) { throw new Exception("结果不正确: " + resultPrefx + msg); }
        }
        /// <summary>
        ///     读取结果，返回int
        /// </summary>
        private int ReadInt()
        {
            var resultPrefx = ReadPrefx();
            var msg = ReadString();

            if (resultPrefx == '-') { throw new Exception(msg.StartsWith("ERR") ? msg.Substring(4) : msg); }
            if (resultPrefx == ':' || resultPrefx == '$') //really strange why ZRANK needs the '$' here
            {
                int i;
                if (int.TryParse(msg, out i)) { return i; }
            }
            throw new Exception("Unknown reply on integer response: " + resultPrefx + msg);
        }
        /// <summary>
        ///     读取结果，返回long
        /// </summary>
        private long ReadLong()
        {
            var resultPrefx = ReadPrefx();
            var msg = ReadString();

            if (resultPrefx == '-') { throw new Exception(msg.StartsWith("ERR") ? msg.Substring(4) : msg); }
            if (resultPrefx == ':' || resultPrefx == '$') //really strange why ZRANK needs the '$' here
            {
                long i;
                if (long.TryParse(msg, out i)) { return i; }
            }
            throw new Exception("Unknown reply on integer response: " + resultPrefx + msg);
        }
        /// <summary>
        ///     读取结果，返回对象数组
        /// </summary>
        private object[] ReadDeeplyNestedMultiData()
        {
            return (object[])ReadDeeplyNestedMultiDataItem();
        }
        /// <summary>
        ///     读取结果，返回对象
        /// </summary>
        private object ReadDeeplyNestedMultiDataItem()
        {
            var resultPrefx = (char)ReadPrefx();
            var msg = ReadString();

            switch (resultPrefx)
            {
                case '$':
                    return ParseSingleLine(string.Concat(char.ToString((char)resultPrefx), msg));

                case '-': throw new Exception("Unknown reply on integer response: " + resultPrefx + msg);

                case '*':
                    int count;
                    if (int.TryParse(msg, out count))
                    {
                        var array = new object[count];
                        for (var i = 0; i < count; i++) { array[i] = ReadDeeplyNestedMultiDataItem(); }

                        return array;
                    }
                    break;

                default: return msg;
            }

            throw new Exception("Unknown reply on integer response: " + resultPrefx + msg);
        }
        /// <summary>
        ///     读取结果，返回字节数组数量
        /// </summary>
        private int ReadMultiDataResultCount()
        {
            var resultPrefx = ReadPrefx();
            var msg = ReadString();

            if (resultPrefx == '-') { throw new Exception(msg.StartsWith("ERR") ? msg.Substring(4) : msg); }
            if (resultPrefx == '*')
            {
                int count;
                if (int.TryParse(msg, out count)) { return count; }
            }
            throw new Exception("Unknown reply on integer response: " + resultPrefx + msg);
        }
        /// <summary>
        ///     读取结果，返回字节数组
        /// </summary>
        private double ReadDouble()
        {
            var bytes = ReadByte();
            return (bytes == null) ? double.NaN : ParseDouble(bytes);
        }
        /// <summary>
        ///     消息转换成Double
        /// </summary>
        /// <param name="msg">消息</param>
        private static double ParseDouble(byte[] msg)
        {
            var doubleString = Encoding.UTF8.GetString(msg);

            double d;
            double.TryParse(doubleString, NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out d);

            return d;
        }

        /// <summary>
        ///     读取结果（消息前缀）
        /// </summary>
        private int ReadPrefx()
        {
            var resultPrefx = _socketStream.ReadByte();
            Check.IsTure(resultPrefx == -1, "没有接收到数据");
            return (char)resultPrefx;
        }

        /// <summary>
        ///     消息转换成字节
        /// </summary>
        /// <param name="msg">消息</param>
        private byte[] ParseSingleLine(string msg)
        {
            Check.IsTure(msg.Length == 0, "没有接收到数据");
            var msgPrefx = msg[0];
            switch (msgPrefx)
            {
                case '$':
                    if (msg == "$-1") { return null; }
                    int count;

                    if (int.TryParse(msg.Substring(1), out count))
                    {
                        var retbuf = new byte[count];
                        var offset = 0;
                        while (count > 0)
                        {
                            var readCount = _socketStream.Read(retbuf, offset, count);
                            Check.IsTure(readCount <= 0, "Unexpected end of Stream");

                            offset += readCount;
                            count -= readCount;
                        }

                        Check.IsTure(_socketStream.ReadByte() != '\r' || _socketStream.ReadByte() != '\n', "Invalid termination");

                        return retbuf;
                    }
                    throw new Exception("Invalid length");
                case ':':
                    return msg.Substring(1).ToUtf8Bytes(); //match the return value
            }

            throw new Exception("Unexpected reply: " + msg);
        }

        #endregion

        #region Redis服务器连接管理

        /// <summary>
        ///     是否已连接
        /// </summary>
        private bool IsConnected()
        {
            try { return !(_socket.Poll(1, SelectMode.SelectRead) && _socket.Available == 0); }
            catch (SocketException)
            {
                return false;
            }
        }

        /// <summary>
        ///     连接Redis服务端
        /// </summary>
        private void Connect()
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp) { SendTimeout = RedisConfigs.ConfigEntity.SendTimeout, ReceiveTimeout = RedisConfigs.ConfigEntity.ReceiveTimeout };
            try
            {
                // 连接到Redis服务端，根据是否设置了连接超时机制决定是否采用异步请求
                if (RedisConfigs.ConfigEntity.ConnectTimeout < 1) { _socket.Connect(_redisConnection.IP, _redisConnection.Port); }
                else
                {
                    var connectResult = _socket.BeginConnect(_redisConnection.IP, _redisConnection.Port, null, null);
                    connectResult.AsyncWaitHandle.WaitOne(RedisConfigs.ConfigEntity.ConnectTimeout, true);
                }
                // 连接失败
                if (!_socket.Connected)
                {
                    _socket.Close();
                    _socket = null;
                    _isError = true;
                    return;
                }
                _socketStream = new BufferedStream(new NetworkStream(_socket), 16 * 1024);
                var redisConn = new RedisConnection(this);
                // 如果包含密码，则发送验证
                if (!string.IsNullOrEmpty(_redisConnection.Password)) { redisConn.Auth(_redisConnection.Password); }
                // 指定库时进行选择
                if (_redisConnection.Db != 0) { redisConn.SelectDb(_redisConnection.Db); }

                //var ipEndpoint = ClientSocket.LocalEndPoint as IPEndPoint;
                //clientPort = ipEndpoint?.Port ?? -1;
                _lastCommand = null;
                _lastConnectedAtTimestamp = Stopwatch.GetTimestamp();
            }
            catch (SocketException ex)
            {
                _socket?.Close();
                _socket = null;

                _isError = true;
                throw new Exception($"连接出错了： IP:{_redisConnection.IP}:{_redisConnection.Port}", ex);
            }
        }

        /// <summary>
        ///     连接Redis服务端，并重置闲置时间
        /// </summary>
        /// <returns></returns>
        private bool ConnectedAndReset()
        {
            if (_lastConnectedAtTimestamp > 0)
            {
                var now = Stopwatch.GetTimestamp();
                // 得到秒单位
                var elapsedSecs = (now - _lastConnectedAtTimestamp) / Stopwatch.Frequency;
                // 闲置时间超时，则重连
                if (elapsedSecs > RedisConfigs.ConfigEntity.IdleTimeOutSecs && !IsConnected()) { return Reconnect(); }
                // 刷新闲置时间
                _lastConnectedAtTimestamp = now;
            }

            if (_socket == null) { Connect(); }

            return _socket != null;
        }

        /// <summary>
        ///     重新连接
        /// </summary>
        /// <returns></returns>
        private bool Reconnect()
        {
            var previousDb = _db;
            Close();
            Connect();

            if (previousDb != _redisConnection.Db) this._db = previousDb;

            return _socket != null;
        }

        /// <summary>
        ///     关闭连接
        /// </summary>
        private void Close()
        {
            // workaround for a .net bug: http://support.microsoft.com/kb/821625
            try { _socketStream?.Close(); }
            finally { _socketStream = null; }

            try { _socket?.Close(); }
            finally { _socket = null; }
        }

        #endregion

        #region 释放资源

        /// <summary>
        ///     释放资源
        /// </summary>
        /// <param name="disposing">是否释放托管资源</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void Dispose(bool disposing)
        {
            //释放托管资源
            if (disposing)
            {
                Close();
                _bufferManger.Dispose();
            }
        }

        /// <summary>
        ///     释放资源
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}