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
    /// Connection（连接）
    /// </summary>
    public class RedisConnection
    {
        /// <summary>
        /// Redis客户端
        /// </summary>
        private readonly RedisClient _redisClient;
        private RedisConnection() { }
        /// <summary>
        /// Connection（连接）
        /// </summary>
        /// <param name="redisClient">Redis客户端</param>
        internal RedisConnection(RedisClient redisClient)
        {
            _redisClient = redisClient;
        }

        /// <summary>
        /// 向Redis发送数据库密码验证
        /// </summary>
        /// <param name="password">数据库密码</param>
        public void Auth(string password)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(password), $"参数：{nameof(password)}不能为空");
            _redisClient.SendExpectSuccess(Commands.Auth, password.ToUtf8Bytes());
        }

        /// <summary>
        /// 切换到指定的数据库，数据库索引号 index 用数字值指定，以 0 作为起始索引值
        /// </summary>
        /// <param name="db">库索引</param>
        public void SelectDb(long db)
        {
            _redisClient.SendExpectSuccess(Commands.Select, db.ToUtf8Bytes());
        }

        /// <summary>
        /// 打印一个特定的信息 message ，测试时使用。
        /// </summary>
        /// <param name="msg">message</param>
        public string Echo(string msg)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(msg), $"参数：{nameof(msg)}不能为空");
            return _redisClient.SendExpectString(Commands.Echo, msg.ToUtf8Bytes());
        }

        /// <summary>
        /// 使用客户端向 Redis 服务器发送一个 PING ，如果服务器运作正常的话，会返回一个 PONG 。
        /// </summary>
        public string Ping()
        {
            return _redisClient.SendExpectString(Commands.Ping);
        }

        /// <summary>
        /// 请求服务器关闭与当前客户端的连接。
        /// </summary>
        public void Close()
        {
            _redisClient.SendExpectString(Commands.Quit);
        }
    }
}
