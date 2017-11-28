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
    /// Hash（哈希表）
    /// </summary>
    public class RedisHash
    {
        /// <summary>
        /// Redis客户端
        /// </summary>
        private readonly RedisClient _redisClient;
        private RedisHash() { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="redisClient">Redis客户端</param>
        internal RedisHash(RedisClient redisClient)
        {
            _redisClient = redisClient;
        }

        /// <summary>
        /// 删除哈希表 key 中的一个或多个指定域，不存在的域将被忽略。
        /// </summary>
        /// <param name="keyName">Key名称</param>
        /// <param name="fields">一个或多个指定域</param>
        /// <returns>被成功移除的域的数量，不包括被忽略的域。</returns>
        public int DeleteFields(string keyName, params string[] fields)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(keyName), $"参数：{nameof(keyName)}不能为空");
            Check.IsTure(fields == null || fields.Length == 0, "参数：fields不能为空");
            var lstCmdBytes = new List<byte[]> { Commands.HDel, keyName.ToUtf8Bytes() };
            lstCmdBytes.AddRange(fields.ToUtf8Bytes());
            return _redisClient.SendExpectInt(lstCmdBytes.ToArray());
        }

        /// <summary> 查看哈希表 key 中，给定域 field 是否存在。 </summary> 
        /// <param name="keyName">Key名称</param>
        /// <param name="fieldName">域名称</param>
        /// <returns>如果哈希表含有给定域，返回 1 。如果哈希表不含有给定域，或 key 不存在，返回 0 。</returns>
        public bool ContainsFields(string keyName, string fieldName)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(keyName), $"参数：{nameof(keyName)}不能为空");
            Check.IsTure(string.IsNullOrWhiteSpace(fieldName), "参数：fieldName不能为空");
            return _redisClient.SendExpectInt(Commands.HExists, keyName.ToUtf8Bytes(), fieldName.ToUtf8Bytes()) > 0;
        }

        /// <summary> 返回哈希表 key 中给定域 field 的值。 </summary> 
        /// <param name="keyName">Key名称</param>
        /// <param name="fieldName">域名称</param>
        /// <returns>当给定域不存在或是给定 key 不存在时，返回 nil 。</returns>
        public string Get(string keyName, string fieldName)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(keyName), $"参数：{nameof(keyName)}不能为空");
            Check.IsTure(string.IsNullOrWhiteSpace(fieldName), "参数：fieldName不能为空");
            return _redisClient.SendExpectString(Commands.HGet, keyName.ToUtf8Bytes(), fieldName.ToUtf8Bytes());
        }

        /// <summary> 返回哈希表 key 中，所有的域和值。 </summary> 
        /// <param name="keyName">Key名称</param>
        /// <returns>以列表形式返回哈希表的域和域的值。</returns>
        public List<KeyValue> Get(string keyName)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(keyName), $"参数：{nameof(keyName)}不能为空");
            var lst = _redisClient.SendExpectMultiData(Commands.HGetAll, keyName.ToUtf8Bytes()).ToListByUtf8<string>();
            return KeyValue.ToList(lst);
        }

        /// <summary>
        /// 为哈希表 key 中的域 field 的值加上增量 addNum 。
        /// 如果 key 不存在，一个新的哈希表被创建并执行 HINCRBY 命令。
        /// 如果域 field 不存在，那么在执行命令前，域的值被初始化为 0 。
        /// </summary>
        /// <param name="keyName">Key名称</param>
        /// <param name="fieldName">域名称</param>
        /// <param name="addNum">增量也可以为负数，相当于对给定域进行减法操作。</param>
        /// <returns></returns>
        public int Add(string keyName, string fieldName, int addNum)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(keyName), $"参数：{nameof(keyName)}不能为空");
            Check.IsTure(string.IsNullOrWhiteSpace(fieldName), "参数：fieldName不能为空");

            return _redisClient.SendExpectInt(Commands.HIncrBy, keyName.ToUtf8Bytes(), fieldName.ToUtf8Bytes(), addNum.ToUtf8Bytes());
        }

        /// <summary>
        /// 为哈希表 key 中的域 field 的值加上增量 addNum 。
        /// 如果 key 不存在，一个新的哈希表被创建并执行 HINCRBY 命令。
        /// 如果域 field 不存在，那么在执行命令前，域的值被初始化为 0 。
        /// </summary>
        /// <param name="keyName">Key名称</param>
        /// <param name="fieldName">域名称</param>
        /// <param name="addNum">增量也可以为负数，相当于对给定域进行减法操作。</param>
        /// <returns></returns>
        public double Add(string keyName, string fieldName, double addNum)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(keyName), $"参数：{nameof(keyName)}不能为空");
            Check.IsTure(string.IsNullOrWhiteSpace(fieldName), "参数：fieldName不能为空");

            return _redisClient.SendExpectLong(Commands.HIncrByFloat, keyName.ToUtf8Bytes(), fieldName.ToUtf8Bytes(), addNum.ToUtf8Bytes());
        }

        /// <summary> 返回哈希表 key 中的所有域。 </summary> 
        /// <param name="keyName">Key名称</param>
        /// <returns>一个包含哈希表中所有域的表。</returns>
        public List<string> GetFields(string keyName)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(keyName), $"参数：{nameof(keyName)}不能为空");
            return _redisClient.SendExpectMultiData(Commands.HKeys, keyName.ToUtf8Bytes()).ToListByUtf8<string>();
        }

        /// <summary> 返回哈希表 key 中域的数量。 </summary> 
        /// <param name="keyName">Key名称</param>
        /// <returns>哈希表中域的数量。</returns>
        public int GetFieldsCount(string keyName)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(keyName), $"参数：{nameof(keyName)}不能为空");
            return _redisClient.SendExpectInt(Commands.HLen, keyName.ToUtf8Bytes());
        }

        /// <summary> 返回哈希表 key 中给定域 field 的值。 </summary> 
        /// <param name="keyName">Key名称</param>
        /// <param name="fields">一个或多个指定域</param>
        /// <returns>当给定域不存在或是给定 key 不存在时，返回 nil 。</returns>
        public List<string> GetList(string keyName, params string[] fields)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(keyName), $"参数：{nameof(keyName)}不能为空");
            Check.IsTure(fields == null || fields.Length == 0, "参数：fields不能为空");

            var lstCmdBytes = new List<byte[]> { Commands.HMGet, keyName.ToUtf8Bytes() };
            lstCmdBytes.AddRange(fields.ToUtf8Bytes());


            return _redisClient.SendExpectMultiData(lstCmdBytes.ToArray()).ToListByUtf8<string>();
        }

        /// <summary>
        /// 批量对多个Key设置值
        /// </summary>
        /// <param name="keyName">Key名称</param>
        /// <param name="keyVals">Key:要设置的Field,Value:要设置的值</param>
        /// <remarks>总是返回 OK (因为 MSET 不可能失败)</remarks>
        public void Set(string keyName, Dictionary<string, string> keyVals)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(keyName), $"参数：{nameof(keyName)}不能为空");
            Check.IsTure(keyVals == null || keyVals.Count == 0, "参数：keyVal不能为空");
            var lstCmdBytes = new List<byte[]> { Commands.HMSet };
            foreach (var keyVal in keyVals)
            {
                lstCmdBytes.Add(keyVal.Key.ToUtf8Bytes());
                lstCmdBytes.Add(keyVal.Value.ToUtf8Bytes());
            }
            _redisClient.SendExpectOk(lstCmdBytes.ToArray());
        }

        /// <summary>
        /// 将哈希表 key 中的域 field 的值设为 value 。
        /// </summary>
        /// <param name="keyName">Key名称</param>
        /// <param name="fieldName">域名称</param>
        /// <param name="val">要设置的值</param>
        public void Set(string keyName, string fieldName, string val)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(keyName), $"参数：{nameof(keyName)}不能为空");
            Check.IsTure(string.IsNullOrWhiteSpace(fieldName), "参数：fieldName不能为空");

            _redisClient.SendExpectSuccess(Commands.HSet, keyName.ToUtf8Bytes(), fieldName.ToUtf8Bytes(), val.ToUtf8Bytes());
        }
        /// <summary>
        /// 将哈希表 key 中的域 field 的值设置为 value ，当且仅当域 field 不存在。
        /// </summary>
        /// <param name="keyName">Key名称</param>
        /// <param name="fieldName">域名称</param>
        /// <param name="val">要设置的值</param>
        public bool SetNx(string keyName, string fieldName, string val)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(keyName), $"参数：{nameof(keyName)}不能为空");
            Check.IsTure(string.IsNullOrWhiteSpace(fieldName), "参数：fieldName不能为空");

            return _redisClient.SendExpectInt(Commands.HSetNx, keyName.ToUtf8Bytes(), fieldName.ToUtf8Bytes(), val.ToUtf8Bytes()) > 0;
        }
    }
}
