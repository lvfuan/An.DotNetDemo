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
    /// List（列表）
    /// </summary>
    public class RedisList
    {
        /// <summary>
        /// Redis客户端
        /// </summary>
        private readonly RedisClient _redisClient;
        private RedisList() { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="redisClient">Redis客户端</param>
        internal RedisList(RedisClient redisClient)
        {
            _redisClient = redisClient;
        }

        /// <summary> 移除并返回列表 key 的头元素。 </summary>
        /// <param name="keyName">Key名称</param>
        /// <returns>列表的头元素。当 key 不存在时，返回 nil 。</returns>
        public string Pop(string keyName)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(keyName), $"参数：{nameof(keyName)}不能为空");
            return _redisClient.SendExpectString(Commands.LPop, keyName.ToUtf8Bytes());
        }

        /// <summary> 移除并返回列表 key 的头元素。当给定列表内没有任何元素可供弹出的时候，连接将被命令阻塞，直到等待超时或发现可弹出元素为止。 </summary>
        /// <param name="timeout">当阻塞时，超时时间,以秒为单位的数字作为值。超时参数设为 0 表示阻塞时间可以无限期延长</param>
        /// <param name="keyName">Key名称</param>
        /// <returns>如果列表为空，返回一个 nil 。否则，返回一个含有两个元素的列表，第一个元素是被弹出元素所属的 key ，第二个元素是被弹出元素的值。</returns>
        public List<KeyValue> Pop(int timeout, params string[] keyName)
        {
            Check.IsTure(keyName == null || keyName.Length == 0, "参数：fields不能为空");
            var lstCmdBytes = new List<byte[]> { Commands.BLPop };
            lstCmdBytes.AddRange(keyName.ToUtf8Bytes());
            lstCmdBytes.Add(timeout.ToUtf8Bytes());
            var lst = _redisClient.SendExpectMultiData(lstCmdBytes.ToArray()).ToListByUtf8<string>();
            return KeyValue.ToList(lst);
        }

        /// <summary> 移除并返回列表 key 的头元素。 </summary>
        /// <param name="keyName">Key名称</param>
        /// <returns>移除并返回列表 key 的尾元素。</returns>
        public string PopLast(string keyName)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(keyName), $"参数：{nameof(keyName)}不能为空");
            return _redisClient.SendExpectString(Commands.RPop, keyName.ToUtf8Bytes());
        }

        /// <summary> 移除并返回列表 key 的尾元素。当给定列表内没有任何元素可供弹出的时候，连接将被命令阻塞，直到等待超时或发现可弹出元素为止。 </summary>
        /// <param name="timeout">当阻塞时，超时时间,以秒为单位的数字作为值。超时参数设为 0 表示阻塞时间可以无限期延长</param>
        /// <param name="keyName">Key名称</param>
        /// <returns>如果列表为空，返回一个 nil 。否则，返回一个含有两个元素的列表，第一个元素是被弹出元素所属的 key ，第二个元素是被弹出元素的值。</returns>
        public List<KeyValue> PopLast(int timeout, params string[] keyName)
        {
            Check.IsTure(keyName == null || keyName.Length == 0, "参数：fields不能为空");
            var lstCmdBytes = new List<byte[]> { Commands.BRPop };
            lstCmdBytes.AddRange(keyName.ToUtf8Bytes());
            lstCmdBytes.Add(timeout.ToUtf8Bytes());
            var lst = _redisClient.SendExpectMultiData(lstCmdBytes.ToArray()).ToListByUtf8<string>();
            return KeyValue.ToList(lst);
        }

        /// <summary>
        /// 将popKeyName进行PopLast操作（末尾弹出），并将pushKeyName进行PushFirst（插入到头元素），最终返回弹出（或插入的）的元素值
        /// </summary>
        /// <param name="popKeyName">末尾弹出的Key</param>
        /// <param name="pushKeyName">插入到头元素的Key</param>
        /// <param name="timeout">当阻塞时，超时时间,以秒为单位的数字作为值。超时参数设为 0 表示阻塞时间可以无限期延长</param>
        /// <returns>假如在指定时间内没有任何元素被弹出，则返回一个 nil 和等待时长。反之，返回一个含有两个元素的列表，第一个元素是被弹出元素的值，第二个元素是等待时长。</returns>
        public string PopLastPushFirst(string popKeyName, string pushKeyName, int timeout)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(popKeyName), "参数：popKeyName不能为空");
            Check.IsTure(string.IsNullOrWhiteSpace(pushKeyName), "参数：pushKeyName不能为空");
            return _redisClient.SendExpectMultiData(Commands.BRPopLPush).ToListByUtf8<string>()[0];
        }
        /// <summary>
        /// 将popKeyName进行PopLast操作（末尾弹出），并将pushKeyName进行PushFirst（插入到头元素），最终返回弹出（或插入的）的元素值
        /// </summary>
        /// <param name="popKeyName">末尾弹出的Key</param>
        /// <param name="pushKeyName">插入到头元素的Key</param>
        /// <returns>被弹出的元素。</returns>
        public string PopLastPushFirst(string popKeyName, string pushKeyName)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(popKeyName), "参数：popKeyName不能为空");
            Check.IsTure(string.IsNullOrWhiteSpace(pushKeyName), "参数：pushKeyName不能为空");
            return _redisClient.SendExpectString(Commands.RPopLPush);
        }

        /// <summary>
        /// 返回列表 key 中，下标为 index 的元素。
        /// </summary>
        /// <param name="keyName">Key名称</param>
        /// <param name="startIndex">以 0 表示列表的第一个元素。以 -1 表示列表的最后一个元素</param>
        /// <returns>如果 index 参数的值不在列表的区间范围内(out of range)，返回 nil</returns>
        public string Get(string keyName, int startIndex)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(keyName), $"参数：{nameof(keyName)}不能为空");
            return _redisClient.SendExpectString(Commands.LIndex, keyName.ToUtf8Bytes(), startIndex.ToUtf8Bytes());
        }

        /// <summary>
        /// 将值 val 插入到列表 key 当中，位于值 beforeVal 之前。
        /// </summary>
        /// <param name="keyName">Key名称</param>
        /// <param name="val">要插入的值</param>
        /// <param name="beforeVal">val插入在beforeVal值的前面，不存在于列表 key 时，不执行任何操作。</param>
        /// <returns>如果命令执行成功，返回插入操作完成之后，列表的长度。如果没有找到 pivot ，返回 -1 。如果 key 不存在或为空列表，返回 0 。</returns>
        public int InsertByBefore(string keyName, string val, string beforeVal)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(keyName), $"参数：{nameof(keyName)}不能为空");
            Check.IsTure(string.IsNullOrWhiteSpace(beforeVal), "参数：keyName不能为空");
            return _redisClient.SendExpectInt(Commands.LInsert, keyName.ToUtf8Bytes(), Commands.Before, beforeVal.ToUtf8Bytes(), val.ToUtf8Bytes());
        }

        /// <summary>
        /// 将值 val 插入到列表 key 当中，位于值 afterVal 之后。
        /// </summary>
        /// <param name="keyName">Key名称</param>
        /// <param name="val">要插入的值</param>
        /// <param name="afterVal">val插入在afterVal值的后面，不存在于列表 key 时，不执行任何操作。</param>
        /// <returns>如果命令执行成功，返回插入操作完成之后，列表的长度。如果没有找到 pivot ，返回 -1 。如果 key 不存在或为空列表，返回 0 。</returns>
        public int InsertByAfter(string keyName, string val, string afterVal)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(keyName), $"参数：{nameof(keyName)}不能为空");
            Check.IsTure(string.IsNullOrWhiteSpace(afterVal), "参数：keyName不能为空");
            return _redisClient.SendExpectInt(Commands.LInsert, keyName.ToUtf8Bytes(), Commands.After, afterVal.ToUtf8Bytes(), val.ToUtf8Bytes());
        }

        /// <summary>
        /// 返回列表 key 的长度
        /// </summary>
        /// <param name="keyName">Key名称</param>
        /// <returns>列表 key 的长度。</returns>
        public int GetCount(string keyName)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(keyName), $"参数：{nameof(keyName)}不能为空");
            return _redisClient.SendExpectInt(Commands.LLen, keyName.ToUtf8Bytes());
        }

        /// <summary> 将一个或多个值 value 插入到列表 key 的表头。 </summary>
        /// <param name="keyName">Key名称</param>
        /// <param name="vals">允许push多个值，value 值按从左到右的顺序依次插入到表头</param>
        /// <returns>返回列表的长度 。</returns>
        public int Push(string keyName, params string[] vals)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(keyName), $"参数：{nameof(keyName)}不能为空");
            var lstCmdBytes = new List<byte[]> { Commands.LPush, keyName.ToUtf8Bytes() };
            lstCmdBytes.AddRange(vals.ToUtf8Bytes());
            return _redisClient.SendExpectInt(lstCmdBytes.ToArray());
        }

        /// <summary> 将一个或多个值 value 插入到列表 key 的表头。 </summary>
        /// <param name="keyName">Key名称</param>
        /// <param name="val">pushvalue 值插入到表头</param>
        /// <returns>返回列表的长度 。</returns>
        public int PushHx(string keyName, string val)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(keyName), $"参数：{nameof(keyName)}不能为空");
            return _redisClient.SendExpectInt(Commands.LPushHx, keyName.ToUtf8Bytes(), val.ToUtf8Bytes());
        }

        /// <summary> 将一个或多个值 value 插入到列表 key 的表尾。 </summary>
        /// <param name="keyName">Key名称</param>
        /// <param name="vals">允许push多个值，value 值按从左到右的顺序依次插入到表尾</param>
        /// <returns>返回列表的长度 。</returns>
        public int Add(string keyName, params string[] vals)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(keyName), $"参数：{nameof(keyName)}不能为空");
            var lstCmdBytes = new List<byte[]> { Commands.RPush, keyName.ToUtf8Bytes() };
            lstCmdBytes.AddRange(vals.ToUtf8Bytes());
            return _redisClient.SendExpectInt(lstCmdBytes.ToArray());
        }

        /// <summary> 将一个或多个值 value 插入到列表 key 的表尾。 </summary>
        /// <param name="keyName">Key名称</param>
        /// <param name="val">pushvalue 值插入到表尾</param>
        /// <returns>返回列表的长度 。</returns>
        public int AddHx(string keyName, string val)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(keyName), $"参数：{nameof(keyName)}不能为空");
            return _redisClient.SendExpectInt(Commands.RPushHx, keyName.ToUtf8Bytes(), val.ToUtf8Bytes());
        }

        /// <summary>
        /// 返回列表 key 中指定区间内的元素，区间以偏移量 startIndex 和 endIndex 指定。
        /// </summary>
        /// <param name="keyName">Key名称</param>
        /// <param name="startIndex">以 0 表示列表的第一个元素，以 -1 表示列表的最后一个元素</param>
        /// <param name="endIndex">以 0 表示列表的第一个元素，以 -1 表示列表的最后一个元素</param>
        /// <returns>一个列表，包含指定区间内的元素。</returns>
        public List<string> Get(string keyName, int startIndex, int endIndex)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(keyName), $"参数：{nameof(keyName)}不能为空");
            return _redisClient.SendExpectMultiData(Commands.LRange, keyName.ToUtf8Bytes(), startIndex.ToUtf8Bytes(), endIndex.ToUtf8Bytes()).ToListByUtf8<string>();
        }

        /// <summary>
        /// 从表头开始向表尾搜索，移除与 value 相等的元素
        /// </summary>
        /// <param name="keyName">Key名称</param>
        /// <param name="val">要移除的值</param>
        /// <returns>被移除元素的数量。</returns>
        public int Remove(string keyName, string val)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(keyName), $"参数：{nameof(keyName)}不能为空");
            return _redisClient.SendExpectInt(Commands.LRem, keyName.ToUtf8Bytes(), 0.ToUtf8Bytes(), val.ToUtf8Bytes());
        }

        /// <summary>
        /// 从表头开始向表尾搜索，移除与 value 相等的元素，数量为 count
        /// </summary>
        /// <param name="keyName">Key名称</param>
        /// <param name="count">要移除的数量，为0时，表示移除全部</param>
        /// <param name="val">要移除的值</param>
        /// <returns>被移除元素的数量。</returns>
        public int RemoveFirst(string keyName, string val, int count = 0)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(keyName), $"参数：{nameof(keyName)}不能为空");
            if (count < 0) { count *= -1; }
            return _redisClient.SendExpectInt(Commands.LRem, keyName.ToUtf8Bytes(), count.ToUtf8Bytes(), val.ToUtf8Bytes());
        }

        /// <summary>
        /// 从表头开始向表尾搜索，移除与 value 相等的元素，数量为 count
        /// </summary>
        /// <param name="keyName">Key名称</param>
        /// <param name="count">要移除的数量，为0时，表示移除全部</param>
        /// <param name="val">要移除的值</param>
        /// <returns>被移除元素的数量。</returns>
        public int RemoveLast(string keyName, string val, int count = 0)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(keyName), $"参数：{nameof(keyName)}不能为空");
            if (count > 0) { count *= -1; }
            return _redisClient.SendExpectInt(Commands.LRem, keyName.ToUtf8Bytes(), count.ToUtf8Bytes(), val.ToUtf8Bytes());
        }

        /// <summary>
        /// 列表只保留指定区间内的元素，不在指定区间之内的元素都将被删除
        /// </summary>
        /// <param name="keyName">Key名称</param>
        /// <param name="startIndex">以 0 表示列表的第一个元素，以 -1 表示列表的最后一个元素</param>
        /// <param name="endIndex">以 0 表示列表的第一个元素，以 -1 表示列表的最后一个元素</param>
        /// <returns>命令执行成功时，返回 ok 。</returns>
        public void Remove(string keyName, int startIndex, int endIndex)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(keyName), $"参数：{nameof(keyName)}不能为空");
            _redisClient.SendExpectOk(Commands.LTrim, keyName.ToUtf8Bytes(), startIndex.ToUtf8Bytes(), endIndex.ToUtf8Bytes());
        }
        /// <summary>
        /// 将列表 key 下标为 index 的元素的值设置为 val 。
        /// </summary>
        /// <param name="keyName">Key名称</param>
        /// <param name="val">要赋值的值</param>
        /// <param name="index">索引</param>
        /// <returns>操作成功返回 ok ，否则返回错误信息。</returns>
        public void Set(string keyName, string val, int index)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(keyName), $"参数：{nameof(keyName)}不能为空");
            _redisClient.SendExpectOk(Commands.LSet, keyName.ToUtf8Bytes(), index.ToUtf8Bytes(), val.ToUtf8Bytes());
        }
    }
}
