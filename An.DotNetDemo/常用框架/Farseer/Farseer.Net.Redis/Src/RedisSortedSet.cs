using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FS.Extends;
using FS.Redis.Data;
using FS.Redis.Internal;
using FS.Utils.Common;

namespace FS.Redis
{
    /// <summary>
    ///  SortedSet（有序集合）
    /// </summary>
    public class RedisSortedSet
    {
        /// <summary>
        /// Redis客户端
        /// </summary>
        private readonly RedisClient _redisClient;
        private RedisSortedSet() { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="redisClient">Redis客户端</param>
        internal RedisSortedSet(RedisClient redisClient)
        {
            _redisClient = redisClient;
        }

        /// <summary>
        /// 将一个或多个 member 元素及其 score 值加入到有序集 key 当中。
        /// </summary>
        /// <param name="keyName">Key名称</param>
        /// <param name="lstScoreValue">SortedSet（有序集合）的值</param>
        /// <returns>被成功添加的新成员的数量，不包括那些被更新的、已经存在的成员。</returns>
        public int Add(string keyName, List<ScoreValue> lstScoreValue)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(keyName), $"参数：{nameof(keyName)}不能为空");
            Check.IsTure(lstScoreValue == null || lstScoreValue.Count == 0, "参数：lstScoreValue不能为空");

            var lstCmdBytes = new List<byte[]> { Commands.ZAdd, keyName.ToUtf8Bytes() };
            foreach (var scoreValue in lstScoreValue)
            {
                lstCmdBytes.Add(scoreValue.Score.ToUtf8Bytes());
                lstCmdBytes.Add(scoreValue.Value.ToUtf8Bytes());
            }
            return _redisClient.SendExpectInt(lstCmdBytes.ToArray());
        }

        /// <summary>
        /// 将一个或多个 member 元素及其 score 值加入到有序集 key 当中。
        /// </summary>
        /// <param name="keyName">Key名称</param>
        /// <param name="member">值</param>
        /// <param name="score">排序权重</param>
        /// <returns>被成功添加的新成员的数量，不包括那些被更新的、已经存在的成员。</returns>
        public bool Add(string keyName, string member, double score)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(keyName), $"参数：{nameof(keyName)}不能为空");
            return _redisClient.SendExpectInt(Commands.ZAdd, keyName.ToUtf8Bytes(), score.ToUtf8Bytes(), member.ToUtf8Bytes()) == 1;
        }

        /// <summary>
        /// 返回集合中元素的数量。
        /// </summary>
        /// <param name="keyName">Key名称</param>
        /// <returns>集合中元素的数量</returns>
        public int GetCount(string keyName)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(keyName), $"参数：{nameof(keyName)}不能为空");
            return _redisClient.SendExpectInt(Commands.ZCard, keyName.ToUtf8Bytes());
        }

        /// <summary>
        /// 返回集合中元素的数量。score 值在 minScore 和 maxScore 之间(默认包括 score 值等于 min 或 max )的成员的数量。
        /// </summary>
        /// <param name="keyName">Key名称</param>
        /// <param name="minScore">最小排序权重</param>
        /// <param name="maxScore">最大排序权重</param>
        /// <returns>score 值在 min 和 max 之间的成员的数量</returns>
        public int GetCount(string keyName, double minScore, double maxScore)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(keyName), $"参数：{nameof(keyName)}不能为空");
            return _redisClient.SendExpectInt(Commands.ZCard, keyName.ToUtf8Bytes());
        }

        /// <summary>
        /// score 值加上增量 increment。
        /// </summary>
        /// <param name="keyName">Key名称</param>
        /// <param name="member">值</param>
        /// <param name="increment">通过传递一个负数值 increment ，让 score 减去相应的值</param>
        /// <returns>member 成员的新 score 值。</returns>
        public double AddScore(string keyName, string member, double increment)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(keyName), $"参数：{nameof(keyName)}不能为空");
            return _redisClient.SendExpectString(Commands.ZIncrBy, keyName.ToUtf8Bytes(), increment.ToUtf8Bytes(), member.ToUtf8Bytes()).ConvertType(0d);
        }

        /// <summary>
        /// 返回有序集 key 中，指定区间内的成员值递增
        /// </summary>
        /// <param name="keyName">Key名称</param>
        /// <param name="startIndex">按 score 值来排序的索引以 0 表示有序集第一个成员，以 -1 表示最后一个成员</param>
        /// <param name="endIndex">按 score 值来排序的索引以 0 表示有序集第一个成员，以 -1 表示最后一个成员</param>
        /// <param name="isDesc">false：按score从小到大；true：按score从大到小</param>
        /// <returns>SortedSet（有序集合）的值、权重</returns>
        public List<ScoreValue> Get(string keyName, int startIndex, int endIndex, bool isDesc = false)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(keyName), $"参数：{nameof(keyName)}不能为空");
            var lst = _redisClient.SendExpectMultiData(isDesc ? Commands.ZRevRange : Commands.ZRange, keyName.ToUtf8Bytes(), startIndex.ToUtf8Bytes(), endIndex.ToUtf8Bytes(), Commands.WithScores).ToListByUtf8<string>();
            return ScoreValue.ToList(lst);
        }

        /// <summary>
        /// 返回有序集 key 中，指定区间内的成员值递增
        /// </summary>
        /// <param name="keyName">Key名称</param>
        /// <param name="startIndex">按 score 值来排序的索引以 0 表示有序集第一个成员，以 -1 表示最后一个成员</param>
        /// <param name="endIndex">按 score 值来排序的索引以 0 表示有序集第一个成员，以 -1 表示最后一个成员</param>
        /// <param name="isDesc">false：按score从小到大；true：按score从大到小</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <returns>SortedSet（有序集合）的值、权重</returns>
        public List<ScoreValue> Get(string keyName, int startIndex, int endIndex, int pageSize, int pageIndex = 1, bool isDesc = false)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(keyName), $"参数：{nameof(keyName)}不能为空");

            var lstCmdBytes = new List<byte[]> { isDesc ? Commands.ZRevRangeByScore : Commands.ZRangeByScore, keyName.ToUtf8Bytes(), startIndex.ToUtf8Bytes(), endIndex.ToUtf8Bytes(), Commands.WithScores };

            // 分页
            if (pageSize > 0)
            {
                lstCmdBytes.Add(Commands.Limit);
                lstCmdBytes.Add((pageIndex > 1 ? pageSize * (pageIndex - 1) : 0).ToUtf8Bytes());
                lstCmdBytes.Add(pageSize.ToUtf8Bytes());
            }
            var lst = _redisClient.SendExpectMultiData(lstCmdBytes.ToArray()).ToListByUtf8<string>();
            return ScoreValue.ToList(lst);
        }

        /// <summary>
        /// 返回有序集 key 中成员 member 的索引
        /// </summary>
        /// <param name="keyName">Key名称</param>
        /// <param name="member">值</param>
        /// <param name="isDesc">false：按score从小到大；true：按score从大到小</param>
        /// <returns>如果 member 是有序集 key 的成员，返回 member 的排名</returns>
        public int GetIndex(string keyName, string member, bool isDesc = false)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(keyName), $"参数：{nameof(keyName)}不能为空");
            return _redisClient.SendExpectInt(isDesc ? Commands.ZRevRank : Commands.ZRank, keyName.ToUtf8Bytes(), member.ToUtf8Bytes());
        }

        /// <summary>
        /// 返回有序集 key 中，成员 member 的 score 值。
        /// </summary>
        /// <param name="keyName">Key名称</param>
        /// <param name="member">值</param>
        /// <returns>member 成员的 score 值，以字符串形式表示</returns>
        public double GetScore(string keyName, string member)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(keyName), $"参数：{nameof(keyName)}不能为空");
            return _redisClient.SendExpectString(Commands.ZScore, keyName.ToUtf8Bytes(), member.ToUtf8Bytes()).ConvertType(0d);
        }

        /// <summary>
        /// 移除有序集合 key 中的一个或多个 member 元素，不存在的 member 元素会被忽略。
        /// </summary>
        /// <param name="keyName">Key名称</param>
        /// <param name="members">要移除的member 元素</param>
        /// <returns>被成功移除的元素的数量，不包括被忽略的元素。</returns>
        public int Remove(string keyName, params string[] members)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(keyName), $"参数：{nameof(keyName)}不能为空");
            Check.IsTure(members == null || members.Length == 0, $"参数：{nameof(members)}不能为空");
            var lstCmdBytes = new List<byte[]> { Commands.ZRem, keyName.ToUtf8Bytes() };
            lstCmdBytes.AddRange(members.ToUtf8Bytes());
            return _redisClient.SendExpectInt(lstCmdBytes.ToArray());
        }

        /// <summary>
        /// 移除有序集 key 中，指定排名(rank)区间内的所有成员。
        /// </summary>
        /// <param name="keyName">Key名称</param>
        /// <param name="startIndex">按 score 值递增(从小到大)来排序的索引以 0 表示有序集第一个成员，以 -1 表示最后一个成员</param>
        /// <param name="endIndex">按 score 值递增(从小到大)来排序的索引以 0 表示有序集第一个成员，以 -1 表示最后一个成员</param>
        /// <returns>被移除成员的数量。</returns>
        public int RemoveIndex(string keyName, int startIndex, int endIndex)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(keyName), $"参数：{nameof(keyName)}不能为空");
            return _redisClient.SendExpectInt(Commands.ZRemRangeByRank, keyName.ToUtf8Bytes(), startIndex.ToUtf8Bytes(), endIndex.ToUtf8Bytes());
        }

        /// <summary>
        /// 移除有序集 key 中，指定排名(rank)区间内的所有成员。
        /// </summary>
        /// <param name="keyName">Key名称</param>
        /// <param name="minScore">最小排序权重</param>
        /// <param name="maxScore">最大排序权重</param>
        /// <returns>被移除成员的数量。</returns>
        public int RemoveScore(string keyName, double minScore, double maxScore)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(keyName), $"参数：{nameof(keyName)}不能为空");
            return _redisClient.SendExpectInt(Commands.ZRemRangeByScore, keyName.ToUtf8Bytes(), minScore.ToUtf8Bytes(), maxScore.ToUtf8Bytes());
        }

        /// <summary>
        /// 将集合 keyNames 的并集 保存到 keyName 中
        /// </summary>
        /// <param name="newKeyName">要保存结果的Key名称</param>
        /// <param name="aggregateType">集合同一个成员多个值的聚合方式</param>
        /// <param name="keyNames">给定的一个或多个有序集的并集</param>
        /// <returns>保存到 newKeyName 的结果集的基数。</returns>
        public int UnionSave(string newKeyName, eumAggregateType aggregateType = eumAggregateType.Sum, params string[] keyNames)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(newKeyName), $"参数：{nameof(newKeyName)}不能为空");
            Check.IsTure(keyNames == null || keyNames.Length < 2, $"参数：{nameof(keyNames)}必须大于1个值");

            var lstCmdBytes = new List<byte[]> { Commands.ZUnionStore, newKeyName.ToUtf8Bytes(), keyNames.Length.ToUtf8Bytes() };
            lstCmdBytes.AddRange(keyNames.ToUtf8Bytes());

            // 聚合方式
            lstCmdBytes.Add(Commands.Aggregate);
            switch (aggregateType)
            {
                case eumAggregateType.Max: lstCmdBytes.Add(Commands.Max); break;
                case eumAggregateType.Min: lstCmdBytes.Add(Commands.Min); break;
                case eumAggregateType.Sum: lstCmdBytes.Add(Commands.Sum); break;
            }
            return _redisClient.SendExpectInt(lstCmdBytes.ToArray());
        }

        /// <summary>
        /// 将集合 keyNames 的交集 保存到 keyName 中
        /// </summary>
        /// <param name="newKeyName">要保存结果的Key名称</param>
        /// <param name="aggregateType">集合同一个成员多个score值的聚合方式</param>
        /// <param name="keyNames">给定的一个或多个有序集的交集</param>
        /// <returns>保存到 newKeyName 的结果集的基数。</returns>
        public int IntersectSave(string newKeyName, eumAggregateType aggregateType = eumAggregateType.Sum, params string[] keyNames)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(newKeyName), $"参数：{nameof(newKeyName)}不能为空");
            Check.IsTure(keyNames == null || keyNames.Length < 2, $"参数：{nameof(keyNames)}必须大于1个值");

            var lstCmdBytes = new List<byte[]> { Commands.ZInterStore, newKeyName.ToUtf8Bytes(), keyNames.Length.ToUtf8Bytes() };
            lstCmdBytes.AddRange(keyNames.ToUtf8Bytes());

            // 聚合方式
            lstCmdBytes.Add(Commands.Aggregate);
            switch (aggregateType)
            {
                case eumAggregateType.Max: lstCmdBytes.Add(Commands.Max); break;
                case eumAggregateType.Min: lstCmdBytes.Add(Commands.Min); break;
                case eumAggregateType.Sum: lstCmdBytes.Add(Commands.Sum); break;
            }
            return _redisClient.SendExpectInt(lstCmdBytes.ToArray());
        }
    }
}
