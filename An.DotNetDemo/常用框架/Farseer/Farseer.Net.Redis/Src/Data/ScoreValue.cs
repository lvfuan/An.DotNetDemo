using System.Collections.Generic;
using FS.Extends;

namespace FS.Redis.Data
{
    /// <summary>
    /// Redis中SortedSet（有序集合）的值、权重
    /// </summary>
    public class ScoreValue
    {
        public ScoreValue(string value, double score)
        {
            Score = score;
            Value = value;
        }

        /// <summary>
        /// 排序权重
        /// </summary>
        public double Score { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 将包含Score、Value的List转换成List<ScoreValue>
        /// </summary>
        /// <param name="lst">包含Score、Value的List</param>
        public static List<ScoreValue> ToList(List<string> lst)
        {
            if (lst == null || lst.Count < 2) { return null; }
            var lstKeyValue = new List<ScoreValue>();
            for (var i = 0; i < lst.Count; i += 2) { lstKeyValue.Add(new ScoreValue(lst[i], lst[i + 1].ConvertType(0d))); }
            return lstKeyValue;
        }
    }
}
