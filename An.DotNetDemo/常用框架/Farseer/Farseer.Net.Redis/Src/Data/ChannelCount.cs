using System.Collections.Generic;
using FS.Extends;

namespace FS.Redis.Data
{
    /// <summary>
    /// 频道名称及订阅数量
    /// </summary>
    public class ChannelCount
    {
        public ChannelCount(string channel, int count)
        {
            SubscribeCount = count;
            Channel = channel;
        }

        /// <summary>
        /// 订阅该频道的客户端数量
        /// </summary>
        public int SubscribeCount { get; set; }
        /// <summary>
        /// 频道名称
        /// </summary>
        public string Channel { get; set; }

        /// <summary>
        /// 将包含Score、Value的List转换成List<ChannelCount>
        /// </summary>
        /// <param name="lst">包含Score、Value的List</param>
        public static List<ChannelCount> ToList(List<string> lst)
        {
            if (lst == null || lst.Count < 2) { return null; }
            var lstKeyValue = new List<ChannelCount>();
            for (var i = 0; i < lst.Count; i += 2) { lstKeyValue.Add(new ChannelCount(lst[i], lst[i + 1].ConvertType(0))); }
            return lstKeyValue;
        }
    }
}
