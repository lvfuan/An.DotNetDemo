using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FS.Redis.Internal;
using FS.Utils.Common;
using FS.Extends;

namespace FS.Redis
{
    /// <summary>
    /// 数字的存储方式
    /// </summary>
    public class RedisNumber
    {
        /// <summary>
        /// Redis客户端
        /// </summary>
        private readonly RedisClient _redisClient;
        private RedisNumber() { }
        /// <summary>
        /// 数字的存储方式
        /// </summary>
        /// <param name="redisClient">Redis客户端</param>
        internal RedisNumber(RedisClient redisClient)
        {
            _redisClient = redisClient;
        }

        /// <summary>
        /// 将 key 中储存的数字值减一
        /// </summary>
        /// <param name="keyName">Key名称</param>
        /// <returns>执行命令之后 key 的值。</returns>
        public int Sub(string keyName)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(keyName), $"参数：{nameof(keyName)}不能为空");
            return _redisClient.SendExpectInt(Commands.Decr, keyName.ToUtf8Bytes());
        }

        /// <summary>
        /// 将 key 中储存的数字值减decrNum
        /// </summary>
        /// <param name="keyName">Key名称</param>
        /// <param name="decrNum">要减去的值</param>
        /// <returns>执行命令之后 key 的值。</returns>
        public int Sub(string keyName,int decrNum)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(keyName), $"参数：{nameof(keyName)}不能为空");
            Check.IsTure(decrNum < 1, "参数：decrNum必须大于0");
            return _redisClient.SendExpectInt(Commands.DecrBy, keyName.ToUtf8Bytes(), decrNum.ToUtf8Bytes());
        }

        /// <summary>
        /// 将 key 中储存的数字值加一
        /// </summary>
        /// <param name="keyName">Key名称</param>
        /// <returns>执行命令之后 key 的值。</returns>
        public int Add(string keyName)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(keyName), $"参数：{nameof(keyName)}不能为空");
            return _redisClient.SendExpectInt(Commands.Incr, keyName.ToUtf8Bytes());
        }

        /// <summary>
        /// 将 key 中储存的数字值加addNum
        /// </summary>
        /// <param name="keyName">Key名称</param>
        /// <param name="addNum">要加的值</param>
        /// <returns>执行命令之后 key 的值。</returns>
        public int Add(string keyName, int addNum)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(keyName), $"参数：{nameof(keyName)}不能为空");
            Check.IsTure(addNum < 1, "参数：addNum必须大于0");
            return _redisClient.SendExpectInt(Commands.IncrBy, keyName.ToUtf8Bytes(), addNum.ToUtf8Bytes());
        }

        /// <summary>
        /// 将 key 中储存的数字值加addNum
        /// </summary>
        /// <param name="keyName">Key名称</param>
        /// <param name="addNum">要加的值</param>
        /// <returns>执行命令之后 key 的值。</returns>
        public double Add(string keyName, double addNum)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(keyName), $"参数：{nameof(keyName)}不能为空");
            Check.IsTure(addNum < 1, "参数：addNum必须大于0");
            return _redisClient.SendExpectDouble(Commands.IncrByFloat, keyName.ToUtf8Bytes(), addNum.ToUtf8Bytes());
        }
    }
}
