using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FS.Extends;
using FS.Redis.Internal;
using FS.Utils.Common;

namespace FS.Redis
{
    /// <summary> Key（键） </summary>
    public class RedisKey
    {
        /// <summary> Redis客户端 </summary>
        private readonly RedisClient _redisClient;
        private RedisKey() { }
        /// <summary> Redis的Key键管理 </summary> 
        /// <param name="redisClient">Redis客户端</param>
        internal RedisKey(RedisClient redisClient)
        {
            _redisClient = redisClient;
        }

        #region Key的基础管理
        /// <summary> 删除给定的一个或多个 key，不存在的 key 会被忽略，返回值：被删除 key 的数量 </summary> 
        /// <param name="keyName">Key名称</param>
        /// <returns>被删除 key 的数量。</returns>
        public int RemoveKey(params string[] keyName)
        {
            Check.IsTure(keyName == null || keyName.Length == 0, "参数：keyName不能为空");
            var lstCmdBytes = new List<byte[]> { Commands.Del };
            lstCmdBytes.AddRange(keyName.ToUtf8Bytes());

            return _redisClient.SendExpectString(lstCmdBytes.ToArray()).ConvertType(0);
        }
        /// <summary> 返回 key 所储存的值的类型 </summary> 
        /// <param name="keyName">Key名称</param>
        /// <returns>none (key不存在)string (字符串)list (列表)set (集合)zset (有序集)hash (哈希表)</returns>
        public string GetType(string keyName)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(keyName), $"参数：{nameof(keyName)}不能为空");
            return _redisClient.SendExpectString(Commands.Type, keyName.ToUtf8Bytes());
        }
        /// <summary> 检查是否存在该Key </summary> 
        /// <param name="pattern">搜索Key支持glob-style的通配符格式：*表示任意一个或多个字符，?表示任意字符，[abc]表示方括号中任意一个字母</param>
        /// <returns>若 key 存在，返回 1 ，否则返回 0 。</returns>
        public bool ContainsKey(string pattern)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(pattern), "参数：pattern不能为空");
            return _redisClient.SendExpectInt(Commands.Exists, pattern.ToUtf8Bytes()) > 0;
        }
        /// <summary> 获取指定Key列表 </summary> 
        /// <param name="pattern">支持glob-style的通配符格式：*表示任意一个或多个字符，?表示任意字符，[abc]表示方括号中任意一个字母</param>
        /// <returns>符合给定模式的 key 列表。</returns>
        public List<string> GetKeys(string pattern = "*")
        {
            Check.IsTure(string.IsNullOrWhiteSpace(pattern), "参数：pattern不能为空");
            return _redisClient.SendExpectMultiData(Commands.Keys, pattern.ToUtf8Bytes()).ToListByUtf8<string>();
        }
        /// <summary> 从当前数据库中随机返回(已使用的)一个key </summary>
        /// <returns>当数据库不为空时，返回一个 key 。当数据库为空时，返回 nil 。</returns>
        public string GetRandomKey() => _redisClient.SendExpectString(Commands.RandomKey);
        /// <summary> 将Key改名，当 newkey 已经存在时， Rename 命令将覆盖旧值。 </summary>
        /// <param name="oldKey">原Key名</param> 
        /// <param name="newKey">新Key名</param>
        /// <returns>改名成功时提示 OK ，失败时候返回一个错误。</returns>
        public void Rename(string oldKey, string newKey)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(oldKey), "参数：oldKey不能为空");
            Check.IsTure(string.IsNullOrWhiteSpace(newKey), "参数：newKey不能为空");
            Check.IsTure(oldKey?.Trim() == newKey?.Trim(), "参数：oldKey、newKey不能是一样的名称");
            _redisClient.SendExpectOk(Commands.Rename, oldKey.ToUtf8Bytes(), newKey.ToUtf8Bytes());
        }
        /// <summary> 当且仅当 newkey 不存在时，将 key 改名为 newkey </summary>
        /// <param name="oldKey">原Key名</param> 
        /// <param name="newKey">新Key名</param>
        /// <returns>修改成功时，返回 1 。如果 newkey 已经存在，返回 0 。</returns>
        public bool RenameNx(string oldKey, string newKey)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(oldKey), "参数：oldKey不能为空");
            Check.IsTure(string.IsNullOrWhiteSpace(newKey), "参数：newKey不能为空");
            Check.IsTure(oldKey?.Trim() == newKey?.Trim(), "参数：oldKey、newKey不能是一样的名称");
            return _redisClient.SendExpectInt(Commands.RenameNx, oldKey.ToUtf8Bytes(), newKey.ToUtf8Bytes()) == 1;
        }

        /// <summary>
        /// 返回当前数据库的 key 的数量。
        /// </summary>
        public int GetCount()
        {
            return _redisClient.SendExpectInt(Commands.DbSize);
        }
        #endregion

        #region Key的高级管理
        /// <summary> 将 key 原子性地从当前实例传送到目标实例的指定数据库上，一旦传送成功， key 保证会出现在目标实例上，而当前实例上的 key 会被删除。执行的时候会阻塞进行迁移的两个实例 </summary> 
        /// <param name="keyName">Key名称</param> <param name="ip">移动目标Redis服务端IP</param> 
        /// <param name="port">移动目标Redis服务端端口</param>
        /// <param name="db">目标库</param>
        /// <param name="timeout">超时时间（毫秒）</param>
        /// <param name="isRemoveLocal">是否移除本地Key</param>
        /// <param name="isReplaceTargetKey">当目标存在Key时，是否替换</param>
        /// <returns>迁移成功时返回 OK ，否则返回相应的错误。</returns>
        public void Migrate(string keyName, string ip, int port, int db = 0, int timeout = 1000, bool isRemoveLocal = true, bool isReplaceTargetKey = true)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(keyName), $"参数：{nameof(keyName)}不能为空");
            var lstCmdBytes = new List<byte[]> { Commands.Migrate, ip.ToUtf8Bytes(), port.ToUtf8Bytes(), keyName.ToUtf8Bytes(), db.ToUtf8Bytes(), timeout.ToUtf8Bytes() };
            // 不移除本地Key
            if (!isRemoveLocal) { lstCmdBytes.Add(Commands.Copy); }
            // 当目标存在Key时，替换目标
            if (isReplaceTargetKey) { lstCmdBytes.Add(Commands.Replace); }

            _redisClient.SendExpectOk(lstCmdBytes.ToArray());
        }
        /// <summary> 将当前数据库的 key 移动到给定的数据库 db 当中 </summary> 
        /// <param name="keyName">Key名称</param> <param name="db">库db</param>
        /// <returns>移动成功返回 1 ，失败则返回 0 。</returns>
        public bool Move(string keyName, int db)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(keyName), $"参数：{nameof(keyName)}不能为空");
            return _redisClient.SendExpectInt(Commands.Move, keyName.ToUtf8Bytes(), db.ToUtf8Bytes()) == 1;
        }
        /// <summary> 返回给定 key 引用所储存的值的次数。此命令主要用于除错 </summary> 
        /// <param name="keyName">Key名称</param>
        /// <returns>引用所储存的值的次数</returns>
        public int GetRefcount(string keyName)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(keyName), $"参数：{nameof(keyName)}不能为空");
            return _redisClient.SendExpectInt(Commands.Object, Commands.ObjectRefcount, keyName.ToUtf8Bytes());
        }
        /// <summary> 返回给定 key 所储存的值所使用的内部表示(representation) </summary> 
        /// <param name="keyName">Key名称</param>
        /// <returns>储存的值所使用的内部表示(representation)</returns>
        public int GetIdletime(string keyName)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(keyName), $"参数：{nameof(keyName)}不能为空");
            return _redisClient.SendExpectInt(Commands.Object, Commands.ObjectIdletime, keyName.ToUtf8Bytes());
        }
        /// <summary> 返回给定 key 自储存以来的空转时间(idle， 没有被读取也没有被写入)，以秒为单位 </summary> 
        /// <param name="keyName">Key名称</param>
        /// <returns>编码类型</returns>
        public string GetEncoding(string keyName)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(keyName), $"参数：{nameof(keyName)}不能为空");
            return _redisClient.SendExpectString(Commands.Object, Commands.ObjectEncoding, keyName.ToUtf8Bytes());
        }
        /// <summary> 反序列化给定的序列化值，并将它和给定的 key 关联 </summary> 
        /// <param name="keyName">Key名称</param>
        /// <param name="val">序列化值</param>
        /// <param name="expireByMilliSeconds">毫秒为单位为 key 设置生存时间；如果 ttl 为 0 ，那么不设置生存时间</param>
        /// <returns>如果反序列化成功那么返回 OK ，否则返回一个错误。</returns>
        public void SerializationContrary(string keyName, string val, int expireByMilliSeconds = 0)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(keyName), $"参数：{nameof(keyName)}不能为空");
            _redisClient.SendExpectOk(Commands.Restore, keyName.ToUtf8Bytes(), expireByMilliSeconds.ToUtf8Bytes(), val.ToUtf8Bytes());
        }
        /// <summary> 序列化给定 key，返回被序列化的值，使用 RESTORE 命令可以将这个值反序列化为 Redis 键 </summary> 
        /// <param name="keyName">Key名称</param>
        /// <returns>如果 key 不存在，那么返回 nil 。否则，返回序列化之后的值。</returns>
        public string Serialization(string keyName)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(keyName), $"参数：{nameof(keyName)}不能为空");
            return _redisClient.SendExpectString(Commands.Dump, keyName.ToUtf8Bytes());
        }
        #endregion

    }
}