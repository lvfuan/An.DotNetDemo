using System;
using System.Collections.Generic;
using System.Linq;
using FS.Extends;
using FS.Redis.Data;
using FS.Redis.Internal;
using FS.Utils.Common;

namespace FS.Redis
{
    /// <summary>
    ///     Pub/Sub（发布/订阅）
    /// </summary>
    public class RedisPubSub
    {
        /// <summary>
        ///     Redis客户端
        /// </summary>
        private readonly RedisClient _redisClient;

        private RedisPubSub()
        {
        }
        /// <summary>
        /// </summary>
        /// <param name="redisClient">Redis客户端</param>
        internal RedisPubSub(RedisClient redisClient)
        {
            _redisClient = redisClient;
        }

        /// <summary>
        ///     订阅一个或多个符合给定模式的频道。
        /// </summary>
        /// <param name="patterns">每个模式以 * 作为匹配符，比如 it* 匹配所有以 it 开头的频道</param>
        public void Subscribe(params string[] patterns)
        {
            Check.IsTure(patterns == null || patterns.Length < 2, "参数：patterns必须大于1个值");
            var lstCmdBytes = new List<byte[]> { patterns.Any(o => o.IndexOf('*') > -1) ? Commands.PSubscribe : Commands.Subscribe };
            lstCmdBytes.AddRange(patterns.ToUtf8Bytes());
            new RedisSubscribe(_redisClient).Subscribe(_redisClient.SendExpectMultiData(lstCmdBytes.ToArray()));
        }


        /// <summary>
        ///     将信息 message 发送到指定的频道 channel 。
        /// </summary>
        /// <param name="channel">频道</param>
        /// <param name="message">信息</param>
        /// <returns>接收到信息 message 的订阅者数量。</returns>
        public int Publish(string channel, string message)
        {
            return _redisClient.SendExpectInt(Commands.Publish, channel.ToUtf8Bytes(), message.ToUtf8Bytes());
        }

        /// <summary>
        ///     列出当前至少有一个订阅者的频道 。(订阅模式的客户端不计算在内)
        /// </summary>
        /// <param name="patterns">列出和给定模式 pattern 相匹配的那些活跃频道,不给出 pattern 参数，那么列出订阅与发布系统中的所有活跃频道。</param>
        /// <returns>一个由活跃频道组成的列表。</returns>
        public List<string> GetSubscribeHotList(string patterns = null)
        {
            var lstCmdBytes = new List<byte[]> { Commands.Pubsub, Commands.Channels };
            if (!string.IsNullOrWhiteSpace(patterns)) { lstCmdBytes.Add(patterns.ToUtf8Bytes()); }

            return _redisClient.SendExpectMultiData(lstCmdBytes.ToArray()).ToListByUtf8<string>();
        }

        /// <summary>
        ///     返回给定频道的订阅者数量 。(订阅模式的客户端不计算在内)
        /// </summary>
        /// <param name="channels">频道的名称</param>
        /// <returns>一个由活跃频道组成的列表。</returns>
        public List<ChannelCount> GetSubscribeCountList(params string[] channels)
        {
            Check.IsTure(channels == null || channels.Length < 2, "参数：channels必须大于1个值");
            var lstCmdBytes = new List<byte[]> { Commands.Pubsub, Commands.NumSub };
            lstCmdBytes.AddRange(channels.ToUtf8Bytes());

            var lst = _redisClient.SendExpectMultiData(lstCmdBytes.ToArray()).ToListByUtf8<string>();
            return ChannelCount.ToList(lst);
        }

        /// <summary>
        ///     返回客户端订阅的所有模式的数量总和。
        /// </summary>
        public int GetTotalSubscribeCount()
        {
            return _redisClient.SendExpectInt(Commands.Pubsub, Commands.NumPat);
        }

        /// <summary>
        ///     退订所有给定模式
        /// </summary>
        /// <param name="patterns">退订给定模式 pattern 相匹配的频道,不给出 pattern 参数，那么退订所有频道。每个模式以 * 作为匹配符，比如 it* 匹配所有以 it 开头的频道</param>
        public void Unsubscribe(string patterns = null)
        {
            var lstCmdBytes = new List<byte[]> { patterns.IndexOf('*') > -1 ? Commands.PUnSubscribe : Commands.UnSubscribe };
            if (!string.IsNullOrWhiteSpace(patterns)) { lstCmdBytes.Add(patterns.ToUtf8Bytes()); }
            _redisClient.SendExpectSuccess(lstCmdBytes.ToArray());
        }
    }
}