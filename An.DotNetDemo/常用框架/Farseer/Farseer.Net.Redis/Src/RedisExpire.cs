using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FS.Extends;
using FS.Redis.Internal;
using FS.Utils.Common;

namespace FS.Redis
{
    /// <summary> Redis的Key有效期 </summary>
    public class RedisExpire
    {
        /// <summary> Redis客户端 </summary>
        private readonly RedisClient _redisClient;
        private RedisExpire() { }
        /// <summary> Redis的Key键管理 </summary> 
        /// <param name="redisClient">Redis客户端</param>
        internal RedisExpire(RedisClient redisClient)
        {
            _redisClient = redisClient;
        }

        /// <summary> 以秒为单位设置 key 的有效时间 </summary> 
        /// <param name="keyName">Key名称</param> 
        /// <param name="seconds">秒单位</param>
        /// <returns></returns>
        public bool SetSeconds(string keyName, int seconds)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(keyName), $"参数：{nameof(keyName)}不能为空");
            return _redisClient.SendExpectInt(Commands.Expire, keyName.ToUtf8Bytes(), seconds.ToUtf8Bytes()) == 1;
        }
        /// <summary> 以毫秒为单位设置 key 的有效时间 </summary> 
        /// <param name="keyName">Key名称</param> 
        /// <param name="milliSeconds">毫秒单位</param>
        /// <returns>设置成功，返回 1，key 不存在或设置失败，返回 0</returns>
        public bool SetMilliSeconds(string keyName, int milliSeconds)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(keyName), $"参数：{nameof(keyName)}不能为空");
            return _redisClient.SendExpectInt(Commands.PExpire, keyName.ToUtf8Bytes(), milliSeconds.ToUtf8Bytes()) == 1;
        }
        /// <summary> 以秒为单位设置 key 的有效时间(timespan) </summary> 
        /// <param name="keyName">Key名称</param> 
        /// <param name="timestampSeconds">时间戳秒单位</param>
        /// <returns>如果生存时间设置成功，返回 1 。当 key 不存在或没办法设置生存时间，返回 0 。</returns>
        public bool SetTimestampSeconds(string keyName, int timestampSeconds)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(keyName), $"参数：{nameof(keyName)}不能为空");
            return _redisClient.SendExpectInt(Commands.ExpireAt, keyName.ToUtf8Bytes(), timestampSeconds.ToUtf8Bytes()) == 1;
        }
        /// <summary> 以毫秒为单位设置 key 的有效时间(timespan) </summary> 
        /// <param name="keyName">Key名称</param> 
        /// <param name="timestampMilliSeconds">时间戳单位毫秒</param>
        /// <returns>如果生存时间设置成功，返回 1 。当 key 不存在或没办法设置生存时间时，返回 0 </returns>
        public bool SetTimestampMilliSeconds(string keyName, int timestampMilliSeconds)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(keyName), $"参数：{nameof(keyName)}不能为空");
            return _redisClient.SendExpectInt(Commands.PExpireAt, keyName.ToUtf8Bytes(), timestampMilliSeconds.ToUtf8Bytes()) == 1;
        }
        /// <summary> 移除给定 key 的有效时间 </summary> 
        /// <param name="keyName">Key名称</param>
        /// <returns>当生存时间移除成功时，返回 1 .如果 key 不存在或 key 没有设置生存时间，返回 0 。</returns>
        public bool Remove(string keyName)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(keyName), $"参数：{nameof(keyName)}不能为空");
            return _redisClient.SendExpectInt(Commands.Persist, keyName.ToUtf8Bytes()) == 1;
        }
        /// <summary> 以毫秒为单位返回key的剩余有效时间 </summary> 
        /// <param name="keyName">Key名称</param>
        /// <returns>当 key 不存在时，返回 -2 。当 key 存在但没有设置剩余生存时间时，返回 -1 。否则，以毫秒为单位，返回 key 的剩余生存时间。</returns>
        public int GetMilliSeconds(string keyName)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(keyName), $"参数：{nameof(keyName)}不能为空");
            return _redisClient.SendExpectInt(Commands.PTtl, keyName.ToUtf8Bytes());
        }
        /// <summary> 以秒为单位返回key的剩余有效时间 </summary> 
        /// <param name="keyName">Key名称</param>
        /// <returns>当 key 不存在时，返回 -2 。当 key 存在但没有设置剩余生存时间时，返回 -1 。否则，以秒为单位，返回 key 的剩余生存时间。</returns>
        public int GetSeconds(string keyName)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(keyName), $"参数：{nameof(keyName)}不能为空");
            return _redisClient.SendExpectInt(Commands.Ttl, keyName.ToUtf8Bytes());
        }
    }
}