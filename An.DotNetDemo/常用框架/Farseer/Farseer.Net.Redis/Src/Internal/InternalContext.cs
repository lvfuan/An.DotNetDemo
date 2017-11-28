using System;
using System.Collections.Generic;
using FS.Utils.Common;
using FS.Extends;

namespace FS.Redis.Internal
{
    /// <summary>
    ///     数据库上下文初始化程序
    /// </summary>
    internal class InternalContext
    {
        /// <summary>
        ///     上下文初始化器（只赋值，不初始化，有可能被重复创建两次）
        /// </summary>
        /// <param name="contextType">外部上下文类型</param>
        /// <param name="redisConnection">上下文数据库连接信息</param>
        public InternalContext(Type contextType, Connection redisConnection)
        {
            this.ContextType = contextType;
            this._redisConnection = redisConnection;
        }
        /// <summary>
        ///     上下文数据库连接信息
        /// </summary>
        private readonly Connection _redisConnection;
        /// <summary>
        /// Redis客户端
        /// </summary>
        private RedisClient _client;
        /// <summary>
        ///     外部上下文类型
        /// </summary>
        public Type ContextType { get; }
        /// <summary>
        ///     是否初始化
        /// </summary>
        public bool IsInitializer { get; private set; }
        /// <summary>
        ///     初始化数据库环境、实例化子类中，所有Set属性
        /// </summary>
        public void Initializer()
        {
            if (IsInitializer) { return; }
            _client = new RedisClient(_redisConnection);
            IsInitializer = true;

            Connection = new RedisConnection(_client);
            Hash = new RedisHash(_client);
            Key = new RedisKey(_client);
            List = new RedisList(_client);
            PubSub = new RedisPubSub(_client);
            Script = new RedisScript(_client);
            Server = new RedisServer(_client);
            Set = new RedisSet(_client);
            SortedSet = new RedisSortedSet(_client);
            String = new RedisString(_client);
            Transaction = new RedisTransaction(_client);
            Bit = new RedisBit(_client);
            Expire = new RedisExpire(_client);
            Sort = new RedisSort(_client);
            Number = new RedisNumber(_client);
        }

        public RedisConnection Connection { get; private set; }
        public RedisHash Hash { get; private set; }
        /// <summary>
        /// Redis的Key键管理
        /// </summary>
        public RedisKey Key { get; private set; }
        public RedisList List { get; private set; }
        public RedisPubSub PubSub { get; private set; }
        public RedisScript Script { get; private set; }
        public RedisServer Server { get; private set; }
        public RedisSet Set { get; private set; }
        public RedisSortedSet SortedSet { get; private set; }
        public RedisString String { get; private set; }
        public RedisTransaction Transaction { get; private set; }
        public RedisBit Bit { get; set; }
        public RedisExpire Expire { get; set; }
        public RedisSort Sort { get; set; }
        public RedisNumber Number { get; set; }
    }
}