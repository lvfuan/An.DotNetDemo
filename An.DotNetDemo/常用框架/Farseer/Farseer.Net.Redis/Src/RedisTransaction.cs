using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FS.Extends;
using FS.Redis.Internal;
using FS.Utils.Common;

namespace FS.Redis
{
    /// <summary>
    /// Transaction（事务）
    /// </summary>
    public class RedisTransaction
    {
        /// <summary>
        /// Redis客户端
        /// </summary>
        private readonly RedisClient _redisClient;
        private RedisTransaction() { }
        /// <summary>
        /// Transaction（事务）
        /// </summary>
        /// <param name="redisClient">Redis客户端</param>
        internal RedisTransaction(RedisClient redisClient)
        {
            _redisClient = redisClient;
        }

        /// <summary>
        ///  监视一个(或多个) key ，如果在事务执行之前这个(或这些) key 被其他命令所改动，那么事务将被打断。
        /// </summary>
        /// <param name="keys">需要被监视的Key</param>
        public void Watch(params string[] keys)
        {
            Check.IsTure(keys == null || keys.Length < 2, $"参数：{nameof(keys)}必须大于1个值");

            var lstCmdBytes = new List<byte[]> { Commands.Watch };
            lstCmdBytes.AddRange(keys.ToUtf8Bytes());
            _redisClient.SendExpectOk(lstCmdBytes.ToArray());
        }

        /// <summary>
        ///  取消对所有 key 的监视。
        /// </summary>
        public void CancelWatch()
        {
            _redisClient.SendExpectOk(Commands.UnWatch);
        }

        /// <summary>
        ///  标记一个事务块的开始。。
        /// </summary>
        public void Start()
        {
            _redisClient.SendExpectOk(Commands.Multi);
        }

        /// <summary>
        ///  执行所有事务块内的命令。
        /// </summary>
        /// <returns>事务块内所有命令的返回值，按命令执行的先后顺序排列。当操作被打断时，返回空值 nil 。</returns>
        public List<string> Commit()
        {
            return _redisClient.SendExpectMultiData(Commands.Exec).ToListByUtf8<string>();
        }

        /// <summary>
        ///  取消事务，放弃执行事务块内的所有命令。
        /// </summary>
        public void CancelCommit()
        {
            _redisClient.SendExpectOk(Commands.Discard);
        }
    }
}
