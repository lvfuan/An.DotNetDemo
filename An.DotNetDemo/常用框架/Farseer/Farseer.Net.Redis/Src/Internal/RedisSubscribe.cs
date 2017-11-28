using System;
using System.Collections.Generic;
using FS.Extends;

namespace FS.Redis.Internal
{
    /// <summary>
    /// 订阅控制
    /// </summary>
    internal class RedisSubscribe
    {
        /// <summary>
        ///     Redis客户端
        /// </summary>
        private readonly RedisClient _redisClient;

        private RedisSubscribe()
        {
        }
        /// <summary>
        /// </summary>
        /// <param name="redisClient">Redis客户端</param>
        internal RedisSubscribe(RedisClient redisClient)
        {
            _redisClient = redisClient;
        }

        /// <summary>
        /// 是否为匹配模式，匹配模式下返回的行数为4，否则为3
        /// </summary>
        private bool _isPSubscription;
        /// <summary>
        /// 订阅的数量
        /// </summary>
        private long _subscriptionCount;
        private List<string> activeChannels;
        private const int MsgIndex = 2;
        private static readonly byte[] SubscribeWord = "subscribe".ToUtf8Bytes();
        private static readonly byte[] PSubscribeWord = "psubscribe".ToUtf8Bytes();
        private static readonly byte[] UnSubscribeWord = "unsubscribe".ToUtf8Bytes();
        private static readonly byte[] PUnSubscribeWord = "punsubscribe".ToUtf8Bytes();
        private static readonly byte[] MessageWord = "message".ToUtf8Bytes();
        private static readonly byte[] PMessageWord = "pmessage".ToUtf8Bytes();


        /// <summary>
        ///     订阅一个或多个符合给定模式的频道。
        /// </summary>
        /// <param name="multiBytes">每个模式以 * 作为匹配符，比如 it* 匹配所有以 it 开头的频道</param>
        public void Subscribe(IList<byte[]> multiBytes)
        {
            ParseSubscriptionResults(multiBytes);

            while (this._subscriptionCount > 0)
            {
                multiBytes = _redisClient.ReadMultiByte();
                ParseSubscriptionResults(multiBytes);
            }
        }
        private void ParseSubscriptionResults(IList<byte[]> multiBytes)
        {
            var componentsPerMsg = _isPSubscription ? 4 : 3;
            for (var i = 0; i < multiBytes.Count; i += componentsPerMsg)
            {
                var messageType = multiBytes[i];
                var channel = multiBytes[i + 1].ToStringByUtf8();
                if (SubscribeWord.IsEqual(messageType) || PSubscribeWord.IsEqual(messageType))
                {
                    _isPSubscription = PSubscribeWord.IsEqual(messageType);

                    this._subscriptionCount = int.Parse(multiBytes[i + MsgIndex].ToStringByUtf8());

                    activeChannels.Add(channel);

                    //if (this.OnSubscribe != null) { this.OnSubscribe(channel); }
                }
                else if (UnSubscribeWord.IsEqual(messageType) || PUnSubscribeWord.IsEqual(messageType))
                {
                    this._subscriptionCount = int.Parse(multiBytes[i + 2].ToStringByUtf8());

                    activeChannels.Remove(channel);

                    //if (this.OnUnSubscribe != null) { this.OnUnSubscribe(channel); }
                }
                else if (MessageWord.IsEqual(messageType))
                {
                    var message = multiBytes[i + MsgIndex].ToStringByUtf8();

                    //if (this.OnMessage != null) { this.OnMessage(channel, message); }
                }
                else if (PMessageWord.IsEqual(messageType))
                {
                    var message = multiBytes[i + MsgIndex + 1].ToStringByUtf8();
                    channel = multiBytes[i + 2].ToStringByUtf8();
                    //if (this.OnMessage != null) { this.OnMessage(channel, message); }
                }
                else
                { throw new Exception("Invalid state. Expected [[p]subscribe|[p]unsubscribe|message] got: " + messageType.ToStringByUtf8()); }
            }
        }
    }
}