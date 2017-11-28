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
    /// Set（集合）
    /// </summary>
    public class RedisSet
    {
        /// <summary>
        /// Redis客户端
        /// </summary>
        private readonly RedisClient _redisClient;
        private RedisSet() { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="redisClient">Redis客户端</param>
        internal RedisSet(RedisClient redisClient)
        {
            _redisClient = redisClient;
        }

        /// <summary>
        /// 一个或多个 member 元素加入到集合 key 当中，已经存在于集合的 member 元素将被忽略
        /// </summary>
        /// <param name="keyName">Key名称</param>
        /// <param name="members">member 元素</param>
        /// <returns>被添加到集合中的新元素的数量，不包括被忽略的元素。</returns>
        public int Add(string keyName, params string[] members)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(keyName), $"参数：{nameof(keyName)}不能为空");
            Check.IsTure(members == null || members.Length == 0, $"参数：{nameof(members)}不能为空");
            var lstCmdBytes = new List<byte[]> { Commands.SAdd, keyName.ToUtf8Bytes() };
            lstCmdBytes.AddRange(members.ToUtf8Bytes());
            return _redisClient.SendExpectInt(lstCmdBytes.ToArray());
        }

        /// <summary>
        /// 返回集合中元素的数量。
        /// </summary>
        /// <param name="keyName">Key名称</param>
        /// <returns>集合中元素的数量</returns>
        public int GetCount(string keyName)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(keyName), $"参数：{nameof(keyName)}不能为空");
            return _redisClient.SendExpectInt(Commands.SCard, keyName.ToUtf8Bytes());
        }

        /// <summary>
        /// 返回集合 keyNames 的差集。
        /// </summary>
        /// <param name="keyNames">给定的一个或多个有序集的差集</param>
        /// <returns>差集列表。</returns>
        public List<string> Except(params string[] keyNames)
        {
            Check.IsTure(keyNames == null || keyNames.Length < 2, $"参数：{nameof(keyNames)}必须大于1个值");

            var lstCmdBytes = new List<byte[]> { Commands.SDiff };
            lstCmdBytes.AddRange(keyNames.ToUtf8Bytes());
            return _redisClient.SendExpectMultiData(lstCmdBytes.ToArray()).ToListByUtf8<string>();
        }

        /// <summary>
        /// 将集合 keyNames 的差集 保存到 keyName 中
        /// </summary>
        /// <param name="newKeyName">要保存结果的Key名称</param>
        /// <param name="keyNames">给定的一个或多个有序集的差集</param>
        /// <returns>集合中元素的数量。</returns>
        public int ExceptSave(string newKeyName, params string[] keyNames)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(newKeyName), $"参数：{nameof(newKeyName)}不能为空");
            Check.IsTure(keyNames == null || keyNames.Length < 2, $"参数：{nameof(keyNames)}必须大于1个值");

            var lstCmdBytes = new List<byte[]> { Commands.SDiffStore, newKeyName.ToUtf8Bytes() };
            lstCmdBytes.AddRange(keyNames.ToUtf8Bytes());
            return _redisClient.SendExpectInt(lstCmdBytes.ToArray());
        }

        /// <summary>
        /// 返回集合 keyNames 的交集。
        /// </summary>
        /// <param name="keyNames">给定的一个或多个有序集的交集</param>
        /// <returns>交集列表</returns>
        public List<string> Intersect(params string[] keyNames)
        {
            Check.IsTure(keyNames == null || keyNames.Length < 2, $"参数：{nameof(keyNames)}必须大于1个值");

            var lstCmdBytes = new List<byte[]> { Commands.SInter };
            lstCmdBytes.AddRange(keyNames.ToUtf8Bytes());
            return _redisClient.SendExpectMultiData(lstCmdBytes.ToArray()).ToListByUtf8<string>();
        }

        /// <summary>
        /// 将集合 keyNames 的交集 保存到 keyName 中
        /// </summary>
        /// <param name="newKeyName">要保存结果的Key名称</param>
        /// <param name="keyNames">给定的一个或多个有序集的交集</param>
        /// <returns>集合中元素的数量。</returns>
        public int IntersectSave(string newKeyName, params string[] keyNames)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(newKeyName), $"参数：{nameof(newKeyName)}不能为空");
            Check.IsTure(keyNames == null || keyNames.Length < 2, $"参数：{nameof(keyNames)}必须大于1个值");

            var lstCmdBytes = new List<byte[]> { Commands.SInterStore, newKeyName.ToUtf8Bytes() };
            lstCmdBytes.AddRange(keyNames.ToUtf8Bytes());
            return _redisClient.SendExpectInt(lstCmdBytes.ToArray());
        }

        /// <summary>
        /// 返回集合 keyNames 的并集。
        /// </summary>
        /// <param name="keyNames">给定的一个或多个有序集的并集</param>
        /// <returns>并集列表</returns>
        public List<string> Union(params string[] keyNames)
        {
            Check.IsTure(keyNames == null || keyNames.Length < 2, $"参数：{nameof(keyNames)}必须大于1个值");

            var lstCmdBytes = new List<byte[]> { Commands.SUnion };
            lstCmdBytes.AddRange(keyNames.ToUtf8Bytes());
            return _redisClient.SendExpectMultiData(lstCmdBytes.ToArray()).ToListByUtf8<string>();
        }

        /// <summary>
        /// 将集合 keyNames 的并集 保存到 keyName 中
        /// </summary>
        /// <param name="newKeyName">要保存结果的Key名称</param>
        /// <param name="keyNames">给定的一个或多个有序集的并集</param>
        /// <returns>集合中元素的数量。</returns>
        public int UnionSave(string newKeyName, params string[] keyNames)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(newKeyName), $"参数：{nameof(newKeyName)}不能为空");
            Check.IsTure(keyNames == null || keyNames.Length < 2, $"参数：{nameof(keyNames)}必须大于1个值");

            var lstCmdBytes = new List<byte[]> { Commands.SUnionStore, newKeyName.ToUtf8Bytes() };
            lstCmdBytes.AddRange(keyNames.ToUtf8Bytes());
            return _redisClient.SendExpectInt(lstCmdBytes.ToArray());
        }

        /// <summary> 判断 member 元素是否集合 key 的成员。 </summary> 
        /// <param name="keyName">Key名称</param>
        /// <returns>如果 member 元素是集合的成员，返回 1 。</returns>
        public bool ContainsMember(string keyName)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(keyName), $"参数：{nameof(keyName)}不能为空");
            return _redisClient.SendExpectInt(Commands.SIsMember, keyName.ToUtf8Bytes()) > 0;
        }

        /// <summary> 返回集合 key 中的所有成员。 </summary> 
        /// <param name="keyName">Key名称</param>
        /// <returns>集合中的所有成员。</returns>
        public List<string> Get(string keyName)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(keyName), $"参数：{nameof(keyName)}不能为空");
            return _redisClient.SendExpectMultiData(Commands.SMembers, keyName.ToUtf8Bytes()).ToListByUtf8<string>();
        }

        /// <summary> 将 member 元素从 source 集合移动到 destination 集合。 </summary> 
        /// <param name="sourceKeyName">要移动的Key名称</param>
        /// <param name="member">要移动的member</param>
        /// <param name="destinationKeyName">要保存到目标的Key名称</param>
        /// <returns>如果 member 元素被成功移除，返回 1 。。</returns>
        public bool Move(string sourceKeyName, string member, string destinationKeyName)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(sourceKeyName), $"参数：{nameof(sourceKeyName)}不能为空");
            Check.IsTure(string.IsNullOrWhiteSpace(destinationKeyName), $"参数：{nameof(destinationKeyName)}不能为空");
            return _redisClient.SendExpectInt(Commands.SMove, sourceKeyName.ToUtf8Bytes(), destinationKeyName.ToUtf8Bytes(), member.ToUtf8Bytes()) > 0;
        }

        /// <summary>
        /// 移除并返回集合中的一个随机元素。
        /// </summary>
        /// <param name="keyName">Key名称</param>
        /// <returns>被移除的随机元素。</returns>
        public string PopRand(string keyName)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(keyName), $"参数：{nameof(keyName)}不能为空");
            return _redisClient.SendExpectString(Commands.SPop, keyName.ToUtf8Bytes());
        }

        /// <summary>
        /// 返回一个数组；如果集合为空，返回空数组
        /// </summary>
        /// <param name="keyName">Key名称</param>
        /// <param name="count">
        ///     count 为正数,返回一个包含 count 个元素的数组，数组中的元素各不相同；
        ///     count 为负数返回一个数组，数组中的元素可能会重复出现多次
        /// </param>
        /// <returns>返回一个数组；如果集合为空，返回空数组</returns>
        public List<string> GetRand(string keyName, int count)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(keyName), $"参数：{nameof(keyName)}不能为空");
            return _redisClient.SendExpectMultiData(Commands.SRandMember, keyName.ToUtf8Bytes(), count.ToUtf8Bytes()).ToListByUtf8<string>();
        }

        /// <summary>
        /// 返返回集合中的一个随机元素
        /// </summary>
        /// <param name="keyName">Key名称</param>
        /// <returns>返回集合中的一个随机元素</returns>
        public string GetRand(string keyName)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(keyName), $"参数：{nameof(keyName)}不能为空");
            return _redisClient.SendExpectString(Commands.SRandMember, keyName.ToUtf8Bytes());
        }

        /// <summary>
        /// 移除集合 key 中的一个或多个 member 元素，不存在的 member 元素会被忽略。
        /// </summary>
        /// <param name="keyName">Key名称</param>
        /// <param name="members">要移除的member 元素</param>
        /// <returns>被成功移除的元素的数量，不包括被忽略的元素。</returns>
        public int Remove(string keyName, params string[] members)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(keyName), $"参数：{nameof(keyName)}不能为空");
            Check.IsTure(members == null || members.Length == 0, $"参数：{nameof(members)}不能为空");
            var lstCmdBytes = new List<byte[]> { Commands.SRem, keyName.ToUtf8Bytes() };
            lstCmdBytes.AddRange(members.ToUtf8Bytes());
            return _redisClient.SendExpectInt(lstCmdBytes.ToArray());
        }
    }
}
