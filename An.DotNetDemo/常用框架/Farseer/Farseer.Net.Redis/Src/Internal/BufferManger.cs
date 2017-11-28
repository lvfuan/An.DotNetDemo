using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using FS.Configs;

namespace FS.Redis.Internal
{
    /// <summary>
    /// 缓冲区管理
    /// </summary>
    internal class BufferManger : IDisposable
    {
        /// <summary>
        ///     当前存冲区索引
        /// </summary>
        private int _currentBufferIndex;
        /// <summary>
        ///     当前缓冲池
        /// </summary>
        private byte[] _currentBuffer = BufferPool.GetBuffer();
        /// <summary>
        /// 待发送的缓冲区队列
        /// </summary>
        private readonly IList<ArraySegment<byte>> _lstBuffer = new List<ArraySegment<byte>>();
        /// <summary>
        /// 将当前缓冲区推送到队列中
        /// </summary>
        private void Push()
        {
            _lstBuffer.Add(new ArraySegment<byte>(_currentBuffer, 0, _currentBufferIndex));
            _currentBuffer = BufferPool.GetBuffer();
            _currentBufferIndex = 0;
        }
        /// <summary>
        /// 将缓冲区队列发送到服务端
        /// </summary>
        /// <param name="sock"></param>
        public bool Send(Socket sock)
        {
            try
            {
                // 推送到队列中
                if (_currentBufferIndex > 0) { Push(); }

                //if (!Env.IsMono)
                if (true)
                {
                    sock.Send(_lstBuffer); //Optimized for Windows
                }
                //else
                //{
                //    //Sendling IList<ArraySegment> Throws 'Message to Large' SocketException in Mono
                //    foreach (var segment in _lstBuffer)
                //    {
                //        var buffer = segment.Array;
                //        sock.Send(buffer, segment.Offset, segment.Count, SocketFlags.None);
                //    }
                //}
                Reset();
            }
            catch (SocketException ex)
            {
                _lstBuffer.Clear();
                throw ex;
            }
            return true;
        }
        /// <summary>
        /// 重置当前缓冲区索引及队列
        /// </summary>
        private void Reset()
        {
            _currentBufferIndex = 0;
            for (int i = _lstBuffer.Count - 1; i >= 0; i--)
            {
                var buffer = _lstBuffer[i].Array;
                BufferPool.ReleaseBufferToPool(ref buffer);
                _lstBuffer.RemoveAt(i);
            }
        }
        /// <summary>
        /// 将命令、内容根据Redis协议进行组装并写入缓冲区
        /// </summary>
        /// <param name="cmdWithBinaryArgs">命令、内容</param>
        public void WriteAll(params byte[][] cmdWithBinaryArgs)
        {
            /* 消息头标识，消息行、行数据块大小描述
               Redis是以行来划分，每行以\r\n行结束
               每一行都有一个消息头，消息头共分为5种分别如下:

               (+) 表示一个正确的状态信息，具体信息是当前行 + 后面的字符。
               (-)  表示一个错误信息，具体信息是当前行－后面的字符。
               (*) 表示消息体总共有多少行，不包括当前行,*后面是具体的行数。
               ($) 表示下一行数据长度，不包括换行符长度\r\n,$后面则是对应的长度的数据。
               (:) 表示返回一个数值，：后面是相应的数字节符。*/

            // 消息头标识：消息总行数
            Write(GetCmdLineByte('*', cmdWithBinaryArgs.Length));

            foreach (var safeBinaryValue in cmdWithBinaryArgs)
            {
                // 每行消息头：每行消息的长度
                Write(GetCmdLineByte('$', safeBinaryValue.Length));
                // 消息内容
                Write(safeBinaryValue);
                // 消息结束
                Write(Commands.EndData);
            }
        }
        /// <summary>
        /// 将命令、内容根据Redis协议进行组装并写入缓冲区
        /// </summary>
        /// <param name="cmdBytes"></param>
        private void Write(byte[] cmdBytes)
        {
            if (WriteCurrentBuffer(cmdBytes)) return;
            // 缓冲区已满，先将已有的消息推送出去
            Push();

            if (WriteCurrentBuffer(cmdBytes)) return;

            var bytesCopied = 0;
            while (bytesCopied < cmdBytes.Length)
            {
                var copyOfBytes = BufferPool.GetBuffer();
                var bytesToCopy = Math.Min(cmdBytes.Length - bytesCopied, copyOfBytes.Length);
                Buffer.BlockCopy(cmdBytes, bytesCopied, copyOfBytes, 0, bytesToCopy);
                _lstBuffer.Add(new ArraySegment<byte>(copyOfBytes, 0, bytesToCopy));
                bytesCopied += bytesToCopy;
            }
        }
        /// <summary>
        ///     将消息存入缓冲区中（字节超出最大长度时失败）
        /// </summary>
        /// <param name="cmdBytes">要发送的字节</param>
        private bool WriteCurrentBuffer(byte[] cmdBytes)
        {
            // 当cmdBytes长度 + 待发送缓冲池长度 < 最大长度时
            if (cmdBytes.Length + _currentBufferIndex < RedisConfigs.ConfigEntity.BufferLength)
            {
                // 将当前命令复制到缓冲区中
                Buffer.BlockCopy(cmdBytes, 0, _currentBuffer, _currentBufferIndex, cmdBytes.Length);
                _currentBufferIndex += cmdBytes.Length;
                return true;
            }
            return false;
        }
        /// <summary>
        /// 根据Redis协议组装消息的行数
        /// </summary>
        /// <param name="cmdPrefix">消息前缀，如*、$</param>
        /// <param name="lineLength">消息的行数</param>
        /// <returns></returns>
        private byte[] GetCmdLineByte(char cmdPrefix, int lineLength)
        {
            var strLines = lineLength.ToString();
            var strLinesLength = strLines.Length;
            // 1：命令前缀
            // 2：换行符号：/r、/n
            // strLinesLength：消息行数
            var cmdBytes = new byte[1 + strLinesLength + 2];
            cmdBytes[0] = (byte)cmdPrefix;

            // 在Redis协议中，行数的每个位数，都要单独转成字节
            // 如果23行，则要把2 与 3 分别转成字节，放在2个byte中。
            for (var i = 0; i < strLinesLength; i++) { cmdBytes[i + 1] = (byte)strLines[i]; }

            // 换行
            cmdBytes[1 + strLinesLength] = Commands.EndData[0]; // \r
            cmdBytes[2 + strLinesLength] = Commands.EndData[1]; // \n

            return cmdBytes;
        }

        #region 释放资源
        /// <summary>
        ///     释放资源
        /// </summary>
        /// <param name="disposing">是否释放托管资源</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected virtual void Dispose(bool disposing)
        {
            //释放托管资源
            if (disposing) { _lstBuffer.Clear(); }
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
