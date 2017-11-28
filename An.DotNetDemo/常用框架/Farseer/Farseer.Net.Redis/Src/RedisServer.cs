using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FS.Extends;
using FS.Redis.Data;
using FS.Redis.Internal;
using FS.Utils.Common;

namespace FS.Redis
{
    /// <summary>
    /// Server（服务器）
    /// </summary>
    public class RedisServer
    {
        /// <summary>
        /// Redis客户端
        /// </summary>
        private readonly RedisClient _redisClient;
        private RedisServer() { }
        /// <summary>
        /// Server（服务器）
        /// </summary>
        /// <param name="redisClient">Redis客户端</param>
        internal RedisServer(RedisClient redisClient)
        {
            _redisClient = redisClient;
        }

        /// <summary>
        /// 异步重写当前磁盘的数据到AOF文件。
        /// </summary>
        public string ReSaveDbAsync()
        {
            return _redisClient.SendExpectString(Commands.BgRewriteAof);
        }

        /// <summary>
        /// 以阻塞的方式持久化（保存）当前数据库的数据到磁盘
        /// </summary>
        public string SaveDb()
        {
            return _redisClient.SendExpectString(Commands.Save);
        }

        /// <summary>
        /// 异步持久化（保存）当前数据库的数据到磁盘
        /// </summary>
        public string SaveDbAsync()
        {
            return _redisClient.SendExpectString(Commands.BgSave);
        }

        /// <summary>
        /// 返回连接设置的名字。
        /// </summary>
        public string GetName()
        {
            return _redisClient.SendExpectString(Commands.ClientGetName);
        }

        /// <summary>
        /// 返回设置连接设置的名字。
        /// </summary>
        public string SetName(string clientName)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(clientName), $"参数：{nameof(clientName)}不能为空");

            var lstCmdBytes = new List<byte[]>();
            lstCmdBytes.AddRange(Commands.ClientSetName);
            lstCmdBytes.Add(clientName.ToUtf8Bytes());
            return _redisClient.SendExpectString(lstCmdBytes.ToArray());
        }

        /// <summary>
        /// 关闭地址为 ip:port 的客户端。。
        /// </summary>
        public bool Kill(string ip, int port)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(ip), $"参数：{nameof(ip)}不能为空");
            Check.IsTure(port <= 0, $"参数：{nameof(port)}不能小于0");

            var lstCmdBytes = new List<byte[]>();
            lstCmdBytes.AddRange(Commands.ClientKill);
            lstCmdBytes.Add(($"{ip}:{port}").ToUtf8Bytes());

            return _redisClient.SendExpectString(lstCmdBytes.ToArray()) == "OK";
        }

        /// <summary>
        /// 停止所有客户端。
        /// </summary>
        public string KillAll()
        {
            return _redisClient.SendExpectString(Commands.Shutdown);
        }

        /// <summary>
        /// 以人类可读的格式，返回所有连接到服务器的客户端信息和统计数据。
        /// </summary>
        public List<string> GetClientList()
        {
            return _redisClient.SendExpectMultiData(Commands.ClientList).ToListByUtf8<string>();
        }

        /// <summary>
        /// 接受单个参数 parameter 作为搜索关键字，查找所有匹配的配置参数，s* 命令，服务器就会返回所有以 s 开头的配置参数及参数的值
        /// </summary>
        public List<KeyValue> GetConfig(string parameter = "*")
        {
            Check.IsTure(string.IsNullOrWhiteSpace(parameter), $"参数：{nameof(parameter)}不能为空");
            var lstCmdBytes = new List<byte[]>();
            lstCmdBytes.AddRange(Commands.ConfigGet);
            lstCmdBytes.Add(parameter.ToUtf8Bytes());

            var lst = _redisClient.SendExpectMultiData(lstCmdBytes.ToArray()).ToListByUtf8<string>();
            return KeyValue.ToList(lst);
        }

        /// <summary>
        /// 动态地调整 Redis 服务器的配置(configuration)而无须重启。
        /// </summary>
        public List<KeyValue> SetConfig(string keyName, object val)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(keyName), $"参数：{nameof(keyName)}不能为空");
            Check.IsTure(val == null, $"参数：{nameof(val)}不能为空");
            var lstCmdBytes = new List<byte[]>();
            lstCmdBytes.AddRange(Commands.ConfigSet);

            lstCmdBytes.Add(keyName.ToUtf8Bytes());
            lstCmdBytes.Add(val.ToUtf8Bytes());

            var lst = _redisClient.SendExpectMultiData(lstCmdBytes.ToArray()).ToListByUtf8<string>();
            return KeyValue.ToList(lst);
        }

        /// <summary>
        /// 重置 INFO 命令中的某些统计数据
        /// </summary>
        public void ReStat()
        {
            _redisClient.SendExpectOk(Commands.ConfigResetStat);
        }

        /// <summary>
        /// 将当前Config保存到redis.conf文件中
        /// </summary>
        public void SaveConfig()
        {
            _redisClient.SendExpectOk(Commands.ConfigreWrite);
        }

        /// <summary> 返回有关信息。(调试命令，它不应被客户端所使用。) </summary> 
        /// <param name="keyName">Key名称</param>
        /// <returns>当 key 存在时，返回有关信息。当 key 不存在时，返回一个错误。</returns>
        public string DebugObject(string keyName)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(keyName), $"参数：{nameof(keyName)}不能为空");

            var lstCmdBytes = new List<byte[]>();
            lstCmdBytes.AddRange(Commands.DebugObject);
            lstCmdBytes.Add(keyName.ToUtf8Bytes());

            return _redisClient.SendExpectString(lstCmdBytes.ToArray());
        }

        /// <summary>
        /// 清空整个 Redis 服务器的数据(删除所有数据库的所有 key )。
        /// </summary>
        public void RemoveAll()
        {
            _redisClient.SendExpectOk(Commands.FlushAll);
        }

        /// <summary>
        /// 清空当前数据库中的所有 key。
        /// </summary>
        public void RemoveDb()
        {
            _redisClient.SendExpectOk(Commands.FlushDb);
        }


        /// <summary>
        /// 返回关于 Redis 服务器的各种信息和统计数值。
        /// </summary>
        /// <param name="section">只显示指定的要看的信息</param>
        public List<string> GetInfo(string section = null)
        {
            var lstCmdBytes = new List<byte[]> { Commands.Info };
            lstCmdBytes.AddRange(Commands.ClientSetName);
            if (!string.IsNullOrWhiteSpace(section)) { lstCmdBytes.Add(section.ToUtf8Bytes()); }
            return _redisClient.SendExpectMultiData(lstCmdBytes.ToArray()).ToListByUtf8<string>();
        }

        /// <summary>
        /// 返回最近一次 Redis 成功将数据保存到磁盘上的时间，以 UNIX 时间戳格式表示。
        /// </summary>
        public int GetLastSaveTime()
        {
            return _redisClient.SendExpectInt(Commands.LastSave);
        }

        /// <summary>
        /// 实时打印出 Redis 服务器接收到的命令，调试用。
        /// </summary>
        public List<string> Monitor()
        {
            return _redisClient.SendExpectMultiData(Commands.Monitor).ToListByUtf8<string>();
        }

        /// <summary>
        /// 将当前服务器转变为指定服务器的从属服务器(slave server)。
        /// </summary>
        public void SlaveOf(string ip, int port)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(ip), $"参数：{nameof(ip)}不能为空");
            Check.IsTure(port <= 0, $"参数：{nameof(port)}不能小于0");

            _redisClient.SendExpectString(Commands.SlaveOf, ($"{ip}:{port}").ToUtf8Bytes());
        }

        /// <summary>
        /// 取消从属服务器(slave server)。
        /// </summary>
        public void SlaveOfCancel()
        {
            _redisClient.SendExpectString(Commands.SlaveOf, Commands.No, Commands.One);
        }

        /// <summary>
        /// 查看Redis日志
        /// </summary>
        /// <param name="limit">限制要返回的日志数量</param>
        public List<string> GetLogList(int limit = 0)
        {
            var lstCmdBytes = new List<byte[]> { Commands.Slowlog, Commands.Get };
            if (limit > 0) { lstCmdBytes.Add(limit.ToUtf8Bytes()); }

            return _redisClient.SendExpectMultiData(lstCmdBytes.ToUtf8Bytes()).ToListByUtf8<string>();
        }

        /// <summary>
        /// 清除Redis日志
        /// </summary>
        public void ClearLog()
        {
            _redisClient.SendExpectOk(Commands.Slowlog, Commands.Reset);
        }

        /// <summary>
        /// 查看当前日志的数量。
        /// </summary>
        public int GetLogCount()
        {
            return _redisClient.SendExpectInt(Commands.Slowlog, Commands.Len);
        }

        /// <summary>
        /// 查看当前日志的数量。
        /// </summary>
        /// <returns>Key是当前时间(以 UNIX 时间戳格式表示)，Value是当前这一秒钟已经逝去的微秒数。</returns>
        public KeyValue GetTime()
        {
            return KeyValue.ToEntity(_redisClient.SendExpectMultiData(Commands.Time).ToListByUtf8<string>());
        }

        /// <summary>
        /// 向主数据库服务器同步数据。
        /// 如果主服务器是 Redis 2.8 之前的版本，那么从服务器使用 SYNC 命令来进行同步。
        /// </summary>
        public void Sync()
        {
            _redisClient.SendExpectString(Commands.Sync);
        }

        /// <summary>
        /// 向主数据库服务器同步数据。
        /// 如果主服务器是 Redis 2.8 或以上版本，那么从服务器使用 PSYNC 命令来进行同步。
        /// </summary>
        public void PSync()
        {
            _redisClient.SendExpectString(Commands.Psync, "?".ToUtf8Bytes(), (-1).ToUtf8Bytes());
        }
    }
}
