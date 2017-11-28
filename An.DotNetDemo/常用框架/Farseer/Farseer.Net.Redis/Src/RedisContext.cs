using System;
using System.Collections.Generic;
using System.ComponentModel;
using FS.Configs;
using FS.Extends;
using FS.Redis.Internal;

namespace FS.Redis
{
    /// <summary>
    ///     数据库上下文
    /// </summary>
    public class RedisContext : IDisposable
    {
        /// <summary>
        ///     通过数据库配置，连接数据库
        /// </summary>
        /// <param name="dbIndex">数据库选项</param>
        public RedisContext(int dbIndex = 0) : this(DbConfigs.ConfigEntity.DbList[dbIndex].Server, DbConfigs.ConfigEntity.DbList[dbIndex].Port.ConvertType(0), DbConfigs.ConfigEntity.DbList[dbIndex].PassWord)
        {
        }

        /// <summary>
        ///     通过自定义数据链接符，连接数据库
        /// </summary>
        /// <param name="ip">数据库IP地址</param>
        /// <param name="port">数据库端口</param>
        /// <param name="password">数据库密码（没有可不填）</param>
        /// <param name="db">库</param>
        public RedisContext(string ip, int port= 6379, string password = null, long db = 0)
        {
            _connection = new Connection(ip, port, password, db);
        }

        /// <summary>
        ///     静态实例
        /// </summary>
        internal static TPo Data<TPo>() where TPo : RedisContext, new()
        {
            var newInstance = new TPo();
            // 上下文初始化器
            newInstance._internalContext = new InternalContext(typeof(TPo), newInstance._connection) { };
            newInstance.Init();
            return newInstance;
        }

        /// <summary>
        ///     上下文数据库连接信息
        /// </summary>
        private readonly Connection _connection;

        #region InternalContext上下文初始化器
        /// <summary>
        ///     上下文初始化器
        /// </summary>
        private InternalContext _internalContext;

        /// <summary>
        ///     上下文初始化器
        /// </summary>
        internal InternalContext InternalContext
        {
            get
            {
                if (_internalContext == null)
                {
                    // 上下文初始化器
                    _internalContext = new InternalContext(this.GetType(), _connection);
                    // 初始化上下文
                    Init();
                }
                return _internalContext;
            }
        }

        /// <summary>
        ///     初始化上下文
        /// </summary>
        private void Init()
        {
            _internalContext.Initializer();
        }
        #endregion

        #region 禁用智能提示

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string ToString()
        {
            return base.ToString();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public new Type GetType()
        {
            return base.GetType();
        }

        /// <summary>
        ///     释放资源
        /// </summary>
        /// <param name="disposing">是否释放托管资源</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected virtual void Dispose(bool disposing)
        {
            //释放托管资源
            if (disposing) { }
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

        private RedisConnection Connection => InternalContext.Connection;
        public RedisHash Hash => InternalContext.Hash;
        /// <summary>
        /// Redis的Key键管理
        /// </summary>
        public RedisKey Key => InternalContext.Key;
        public RedisList List => InternalContext.List;
        public RedisPubSub PubSub => InternalContext.PubSub;
        public RedisScript Script => InternalContext.Script;
        public RedisServer Server => InternalContext.Server;
        public RedisSet Set => InternalContext.Set;
        public RedisSortedSet SortedSet => InternalContext.SortedSet;
        public RedisString String => InternalContext.String;
        public RedisTransaction Transaction => InternalContext.Transaction;
        public RedisBit Bit => InternalContext.Bit;
        public RedisExpire Expire => InternalContext.Expire;
        public RedisSort Sort => InternalContext.Sort;
        public RedisNumber Number => InternalContext.Number;
    }
}