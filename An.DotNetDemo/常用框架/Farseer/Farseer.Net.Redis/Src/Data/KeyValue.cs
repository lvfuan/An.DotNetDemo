using System.Collections.Generic;

namespace FS.Redis.Data
{
    /// <summary>
    /// Redis中的值
    /// </summary>
    public class KeyValue
    {
        public KeyValue(string key, string value)
        {
            Key = key;
            Value = value;
        }

        public string Key { get; set; }
        public string Value { get; set; }

        /// <summary>
        /// 将包含Key、Value的List转换成List<KeyValue>
        /// </summary>
        /// <param name="lst">包含Key、Value的List</param>
        /// <returns></returns>
        public static List<KeyValue> ToList(List<string> lst)
        {
            if (lst == null || lst.Count < 2) { return null; }
            var lstKeyValue = new List<KeyValue>();
            for (var i = 0; i < lst.Count; i += 2) { lstKeyValue.Add(new KeyValue(lst[i], lst[i + 1])); }
            return lstKeyValue;
        }
        /// <summary>
        /// 将List<string>转换成KeyValue
        /// </summary>
        /// <param name="lst">包含Key、Value的List</param>
        /// <returns></returns>
        public static KeyValue ToEntity(List<string> lst)
        {
            if (lst == null || lst.Count < 2) { return null; }
            return new KeyValue(lst[0], lst[1]);
        }
    }
}
