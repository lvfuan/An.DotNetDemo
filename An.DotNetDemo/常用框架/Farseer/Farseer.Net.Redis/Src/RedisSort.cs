using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FS.Extends;
using FS.Redis.Internal;
using FS.Utils.Common;

namespace FS.Redis
{
    /// <summary> Redis的排序管理 </summary>
    public class RedisSort
    {
        /// <summary> Redis客户端 </summary>
        private readonly RedisClient _redisClient;
        private RedisSort() { }
        /// <summary> Redis的Key键管理 </summary> 
        /// <param name="redisClient">Redis客户端</param>
        internal RedisSort(RedisClient redisClient)
        {
            _redisClient = redisClient;
        }

        /// <summary> 返回指定Key的值列表（支持分页、Key值的排序）</summary> 
        /// <param name="keyName">Key名称</param>
        /// <param name="isDesc">是否倒序</param>
        /// <param name="isUseAlpha">当值为字符串需要排序时，需设置为true</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <returns>返回或保存给定列表、集合、有序集合 key 中经过排序的元素。</returns>
        public List<TReturn> ToList<TReturn>(string keyName, int pageSize = 0, int pageIndex = 1, bool isDesc = false, bool isUseAlpha = false)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(keyName), $"参数：{nameof(keyName)}不能为空");
            var lstCmdBytes = new List<byte[]> { Commands.Sort, keyName.ToUtf8Bytes() };
            // 倒序
            if (isDesc) { lstCmdBytes.Add(Commands.Desc); }
            // 字符串方式比较
            if (isUseAlpha) { lstCmdBytes.Add(Commands.Alpha); }
            // 分页
            if (pageSize > 0)
            {
                lstCmdBytes.Add(Commands.Limit);
                lstCmdBytes.Add((pageIndex > 1 ? pageSize * (pageIndex - 1) : 0).ToUtf8Bytes());
                lstCmdBytes.Add(pageSize.ToUtf8Bytes());
            }
            return _redisClient.SendExpectMultiData(lstCmdBytes.ToArray()).ToListByUtf8<TReturn>();
        }
        /// <summary> 返回指定Key的值列表（支持分页、Key值的排序）</summary> 
        /// <param name="keyName">Key名称</param>
        /// <param name="isDesc">是否倒序</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="outOrderByKeyName">通过外部的Key值进行排序。outOrderByKeyName必须包含*占位符，*代表keyName的值(请参考：http://doc.redisfans.com/key/sort.html#key）</param>
        /// <param name="pageSize">分页大小</param>
        /// <returns>返回或保存给定列表、集合、有序集合 key 中经过排序的元素。</returns>
        public List<TReturn> ToList<TReturn>(string keyName, string outOrderByKeyName, int pageSize = 0, int pageIndex = 1, bool isDesc = false)
        {
            return ToList<TReturn>(keyName, outOrderByKeyName, null, pageSize, pageIndex, isDesc);
        }
        /// <summary> 返回指定Key的值列表（支持分页、Key值的排序）</summary> 
        /// <param name="keyName">Key名称</param>
        /// <param name="isDesc">是否倒序</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="outOrderByKeyName">通过外部的Key值进行排序。outOrderByKeyName必须包含*占位符，*代表keyName的值(请参考：http://doc.redisfans.com/key/sort.html#key）</param>
        /// <param name="selectKeyNames">必须包含*占位符，*代表keyName的值，如果同时要获取keyName的值，可以传入单个字符串：# 符号(请参考：http://doc.redisfans.com/key/sort.html#id3）</param>
        /// <param name="pageSize">分页大小</param>
        /// <returns>返回或保存给定列表、集合、有序集合 key 中经过排序的元素。</returns>
        public List<TReturn> ToList<TReturn>(string keyName, string outOrderByKeyName, string[] selectKeyNames, int pageSize = 0, int pageIndex = 1, bool isDesc = false)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(keyName), $"参数：{nameof(keyName)}不能为空");
            var lstCmdBytes = new List<byte[]> { Commands.Sort, keyName.ToUtf8Bytes() };
            // 使用外键排序
            if (!string.IsNullOrWhiteSpace(outOrderByKeyName)) { lstCmdBytes.Add(Commands.By); lstCmdBytes.Add(outOrderByKeyName.ToUtf8Bytes()); }
            // 倒序
            if (isDesc) { lstCmdBytes.Add(Commands.Desc); }
            // 显示外键的值
            if (selectKeyNames != null && selectKeyNames.Length > 0)
            {
                foreach (var selectKeyName in selectKeyNames) { lstCmdBytes.Add(Commands.Get); lstCmdBytes.Add(selectKeyName.ToUtf8Bytes()); }
            }
            // 分页
            if (pageSize > 0)
            {
                lstCmdBytes.Add(Commands.Limit);
                lstCmdBytes.Add((pageIndex > 1 ? pageSize * (pageIndex - 1) : 0).ToUtf8Bytes());
                lstCmdBytes.Add(pageSize.ToUtf8Bytes());
            }
            return _redisClient.SendExpectMultiData(lstCmdBytes.ToArray()).ToListByUtf8<TReturn>();
        }

        /// <summary> 返回指定Key的值列表（支持分页、Key值的排序）</summary> 
        /// <param name="keyName">Key名称</param>
        /// <param name="isDesc">是否倒序</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="saveKeyName">保存的新Key名称</param>
        /// <param name="outOrderByKeyName">通过外部的Key值进行排序。outOrderByKeyName必须包含*占位符，*代表keyName的值(请参考：http://doc.redisfans.com/key/sort.html#key）</param>
        /// <param name="pageSize">分页大小</param>
        /// <returns>返回排序结果的元素数量</returns>
        public int Save(string keyName, string saveKeyName, string outOrderByKeyName, int pageSize = 0, int pageIndex = 1, bool isDesc = false)
        {
            return Save(keyName, saveKeyName, outOrderByKeyName, null, pageSize, pageIndex, isDesc);
        }
        /// <summary> 返回指定Key的值列表（支持分页、Key值的排序）</summary> 
        /// <param name="keyName">Key名称</param>
        /// <param name="isDesc">是否倒序</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="saveKeyName">保存的新Key名称</param>
        /// <param name="outOrderByKeyName">通过外部的Key值进行排序。outOrderByKeyName必须包含*占位符，*代表keyName的值(请参考：http://doc.redisfans.com/key/sort.html#key）</param>
        /// <param name="selectKeyNames">必须包含*占位符，*代表keyName的值，如果同时要获取keyName的值，可以传入单个字符串：# 符号(请参考：http://doc.redisfans.com/key/sort.html#id3）</param>
        /// <param name="pageSize">分页大小</param>
        /// <returns>返回排序结果的元素数量</returns>
        public int Save(string keyName, string saveKeyName, string outOrderByKeyName, string[] selectKeyNames, int pageSize = 0, int pageIndex = 1, bool isDesc = false)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(keyName), $"参数：{nameof(keyName)}不能为空");
            var lstCmdBytes = new List<byte[]> { Commands.Sort, keyName.ToUtf8Bytes() };
            // 使用外键排序
            if (!string.IsNullOrWhiteSpace(outOrderByKeyName)) { lstCmdBytes.Add(Commands.By); lstCmdBytes.Add(outOrderByKeyName.ToUtf8Bytes()); }
            // 倒序
            if (isDesc) { lstCmdBytes.Add(Commands.Desc); }
            // 显示外键的值
            if (selectKeyNames != null && selectKeyNames.Length > 0)
            {
                foreach (var selectKeyName in selectKeyNames) { lstCmdBytes.Add(Commands.Get); lstCmdBytes.Add(selectKeyName.ToUtf8Bytes()); }
            }
            // 分页
            if (pageSize > 0)
            {
                lstCmdBytes.Add(Commands.Limit);
                lstCmdBytes.Add((pageIndex > 1 ? pageSize * (pageIndex - 1) : 0).ToUtf8Bytes());
                lstCmdBytes.Add(pageSize.ToUtf8Bytes());
            }
            // 排序结果保存到新的Key
            lstCmdBytes.Add(Commands.Store);
            lstCmdBytes.Add(saveKeyName.ToUtf8Bytes());
            return _redisClient.SendExpectInt(lstCmdBytes.ToArray());
        }
    }
}