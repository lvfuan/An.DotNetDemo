using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FS.Redis.Internal;
using FS.Utils.Common;
using FS.Extends;
using FS.Redis.Data;
using FS.Redis.Infrastructure;

namespace FS.Redis
{
    /// <summary> String（字符串） </summary>
    public class RedisString
    {
        /// <summary>
        /// Redis客户端
        /// </summary>
        private readonly RedisClient _redisClient;
        private RedisString() { }
        /// <summary> Redis的String键管理 </summary>
        /// <param name="redisClient">Redis客户端</param>
        internal RedisString(RedisClient redisClient)
        {
            _redisClient = redisClient;
        }

        /// <summary>
        /// 将 val 追加到 keyName 原来的值的末尾。存在时则以新值方式添加
        /// </summary>
        /// <param name="keyName">Key名称</param>
        /// <param name="val">要追加的值</param>
        /// <returns>追加 value 之后， key 中字符串的长度。</returns>
        public int Append(string keyName, string val)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(keyName), $"参数：{nameof(keyName)}不能为空");
            Check.IsTure(string.IsNullOrWhiteSpace(val), "参数：val不能为空");
            return _redisClient.SendExpectInt(Commands.Append, keyName.ToUtf8Bytes(), val.ToUtf8Bytes());
        }

        /// <summary>
        /// 返回 key 所关联的字符串值。
        /// </summary>
        /// <param name="keyName">Key名称</param>
        /// <returns>当 key 不存在时，返回 nil ，否则，返回 key 的值。如果 key 不是字符串类型，那么返回一个错误。</returns>
        public string Get(string keyName)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(keyName), $"参数：{nameof(keyName)}不能为空");
            return _redisClient.SendExpectString(Commands.Get, keyName.ToUtf8Bytes());
        }

        /// <summary>
        /// 返回 key 中字符串值的子字符串，字符串的截取范围由 start 和 end 两个偏移量决定(包括 start 和 end 在内)。
        /// </summary>
        /// <param name="keyName">Key名称</param>
        /// <param name="startIndex">-1 表示最后一个字符， -2 表示倒数第二个，以此类推</param>
        /// <param name="endIndex">-1 表示最后一个字符， -2 表示倒数第二个，以此类推</param>
        /// <returns>截取得出的子字符串。。</returns>
        public string GetRange(string keyName, int startIndex, int endIndex = -1)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(keyName), $"参数：{nameof(keyName)}不能为空");
            return _redisClient.SendExpectString(Commands.GetRange, keyName.ToUtf8Bytes(), startIndex.ToUtf8Bytes(), endIndex.ToUtf8Bytes());
        }

        /// <summary>
        /// 将给定 key 的值设为 value ，并返回 key 的旧值(old value)。
        /// </summary>
        /// <param name="keyName">Key名称</param>
        /// <param name="newVal">将给定 key 的值设为 newVal ，并返回 key 的旧值(old value)。</param>
        /// <returns>返回给定 key 的旧值。当 key 没有旧值时，也即是， key 不存在时，返回 nil 。</returns>
        public string Get(string keyName, string newVal)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(keyName), $"参数：{nameof(keyName)}不能为空");
            Check.IsTure(newVal == null, "参数：newVal不能为空");
            return _redisClient.SendExpectString(Commands.GetSet, keyName.ToUtf8Bytes(), newVal.ToUtf8Bytes());
        }

        /// <summary>
        /// 返回所有(一个或多个)给定 key 的值。
        /// </summary>
        /// <param name="keyName">Key名称</param>
        /// <returns>一个包含所有给定 key 的值的列表。</returns>
        public List<string> ToList(params string[] keyName)
        {
            Check.IsTure(keyName == null || keyName.Length == 0, "参数：keyName不能为空");
            var lstCmdBytes = new List<byte[]> { Commands.MGet };
            lstCmdBytes.AddRange(keyName.ToUtf8Bytes());
            return _redisClient.SendExpectMultiData(lstCmdBytes.ToArray()).ToListByUtf8<string>();
        }

        /// <summary>
        /// 批量对多个Key设置值
        /// </summary>
        /// <param name="keyVals">Key:要设置的Key,Value:要设置的值</param>
        /// <remarks>总是返回 OK (因为 MSET 不可能失败)</remarks>
        public void Set(Dictionary<string, string> keyVals)
        {
            Check.IsTure(keyVals == null || keyVals.Count == 0, "参数：keyVal不能为空");
            var lstCmdBytes = new List<byte[]> { Commands.MSet };
            foreach (var keyVal in keyVals)
            {
                lstCmdBytes.Add(keyVal.Key.ToUtf8Bytes());
                lstCmdBytes.Add(keyVal.Value.ToUtf8Bytes());
            }
            _redisClient.SendExpectOk(lstCmdBytes.ToArray());
        }

        /// <summary>
        /// 批量对多个Key设置值（原子型：当且仅当所有的Key不存在时，才会成功）
        /// </summary>
        /// <param name="keyVals">Key:要设置的Key,Value:要设置的值</param>
        /// <returns>当所有 key 都成功设置，返回 1 。如果所有给定 key 都设置失败(至少有一个 key 已经存在)，那么返回 0 。</returns>
        public bool SetByNotExists(Dictionary<string, string> keyVals)
        {
            Check.IsTure(keyVals == null || keyVals.Count == 0, "参数：keyVal不能为空");
            var lstCmdBytes = new List<byte[]> { Commands.MSet };
            foreach (var keyVal in keyVals)
            {
                lstCmdBytes.Add(keyVal.Key.ToUtf8Bytes());
                lstCmdBytes.Add(keyVal.Value.ToUtf8Bytes());
            }
            return _redisClient.SendExpectInt(lstCmdBytes.ToArray()) == 0;
        }

        /// <summary>
        /// 将值 value 关联到 key ，并将 key 的生存时间设为 seconds (以秒为单位)。
        /// </summary>
        /// <param name="keyName">Key名称</param>
        /// <param name="val">要赋值的值</param>
        /// <param name="seconds">有效期单位秒</param>
        /// <param name="setType">附加选项</param>
        /// <remarks>设置成功时返回 OK 。当 seconds 参数不合法时，返回一个错误。</remarks>
        public void Set(string keyName, string val, int seconds = 0, eumSetType setType = eumSetType.None)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(keyName), $"参数：{nameof(keyName)}不能为空");
            Check.IsTure(seconds < 1, "参数：seconds必须大于0");
            var lstCmdBytes = new List<byte[]> { Commands.Set, keyName.ToUtf8Bytes(), val.ToUtf8Bytes() };
            // 当设置了秒时
            if (seconds > 0)
            {
                lstCmdBytes.Add(Commands.Ex);
                lstCmdBytes.Add(seconds.ToUtf8Bytes());
            }
            switch (setType)
            {
                case eumSetType.NX: lstCmdBytes.Add(Commands.Nx); break;
                case eumSetType.XX: lstCmdBytes.Add(Commands.Xx); break;
            }
            _redisClient.SendExpectSuccess(lstCmdBytes.ToArray());
        }

        /// <summary>
        /// 将值 value 关联到 key ，并将 key 的生存时间设为 milliSeconds (以毫秒为单位)。
        /// </summary>
        /// <param name="keyName">Key名称</param>
        /// <param name="val">要赋值的值</param>
        /// <param name="milliSeconds">有效期单位毫秒</param>
        /// <param name="setType">附加选项</param>
        /// <remarks>成功完成时，才返回 OK如果设置了 NX 或者 XX ，但因为条件没达到而造成设置操作未执行，那么命令返回空批量回复（NULL Bulk Reply）。</remarks>
        public void SetByMilliSeconds(string keyName, string val, int milliSeconds, eumSetType setType = eumSetType.None)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(keyName), $"参数：{nameof(keyName)}不能为空");
            Check.IsTure(milliSeconds < 1, "参数：milliSeconds必须大于0");
            var lstCmdBytes = new List<byte[]> { Commands.Set, keyName.ToUtf8Bytes(), val.ToUtf8Bytes(), Commands.Px, milliSeconds.ToUtf8Bytes() };
            switch (setType)
            {
                case eumSetType.NX: lstCmdBytes.Add(Commands.Nx); break;
                case eumSetType.XX: lstCmdBytes.Add(Commands.Xx); break;
            }
            _redisClient.SendExpectSuccess(lstCmdBytes.ToArray());
        }

        /// <summary>
        /// 用 value 参数覆写(overwrite)给定 key 所储存的字符串值，从偏移量 offset 开始。
        /// </summary>
        /// <param name="keyName">Key名称</param>
        /// <param name="index">给定 key 所储存的字符串值，从偏移量 offset 开始。</param>
        /// <param name="val">要赋值的值</param>
        /// <returns>修改之后，字符串的长度。</returns>
        public int SetRange(string keyName, string val, int index)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(keyName), $"参数：{nameof(keyName)}不能为空");
            return _redisClient.SendExpectInt(Commands.SetRange, keyName.ToUtf8Bytes(), index.ToUtf8Bytes(), val.ToUtf8Bytes());
        }

        /// <summary>
        /// 返回 key 所储存的字符串值的长度。
        /// </summary>
        /// <param name="keyName">Key名称</param>
        /// <returns>返回 key 所储存的字符串值的长度。</returns>
        public int GetLength(string keyName)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(keyName), $"参数：{nameof(keyName)}不能为空");
            return _redisClient.SendExpectInt(Commands.StrLen, keyName.ToUtf8Bytes());
        }
    }
}
