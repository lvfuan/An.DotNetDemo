using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FS.Redis.Internal;
using FS.Utils.Common;
using FS.Extends;

namespace FS.Redis
{
    /// <summary>
    /// 二进制的存储方式
    /// </summary>
    public class RedisBit
    {
        /// <summary>
        /// Redis客户端
        /// </summary>
        private readonly RedisClient _redisClient;
        private RedisBit() { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="redisClient">Redis客户端</param>
        internal RedisBit(RedisClient redisClient)
        {
            _redisClient = redisClient;
        }

        /// <summary>
        /// 获取被设置为1的数量
        /// </summary>
        /// <param name="keyName">Key名称</param>
        /// <param name="start">0为起始位，负数时以倒数方式往前推</param>
        /// <param name="end">0时不参与计算，负数时以倒数方式往前推</param>
        /// <returns>被设置为 1 的位的数量。</returns>
        public int GetCount(string keyName, int start = 0, int end = 0)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(keyName), $"参数：{nameof(keyName)}不能为空");
            var lstCmdBytes = new List<byte[]> { Commands.BitCount, keyName.ToUtf8Bytes(), start.ToUtf8Bytes() };
            if (end != 0) { lstCmdBytes.Add(end.ToUtf8Bytes()); }
            return _redisClient.SendExpectInt(lstCmdBytes.ToArray());
        }

        /// <summary>
        /// 对一个或多个 keysName 求逻辑并，并将结果保存到 destKeyName 。
        /// </summary>
        /// <param name="destKeyName">保存的新Key</param>
        /// <param name="keysName">要计算的Key</param>
        /// <returns>保存到 destkey 的字符串的长度，和输入 key 中最长的字符串长度相等。</returns>
        public int And(string destKeyName, string[] keysName)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(destKeyName), "参数：destKeyName不能为空");
            Check.IsTure(keysName == null || keysName.Length == 0, "参数：keyName不能为空");

            var lstCmdBytes = new List<byte[]> { Commands.Bitop, Commands.And, destKeyName.ToUtf8Bytes() };
            lstCmdBytes.AddRange(keysName.ToUtf8Bytes());

            return _redisClient.SendExpectInt(lstCmdBytes.ToArray());
        }

        /// <summary>
        /// 对一个或多个 keysName 求逻辑并，并将结果保存到 destKeyName 。
        /// </summary>
        /// <param name="destKeyName">保存的新Key</param>
        /// <param name="keysName">要计算的Key</param>
        /// <returns>保存到 destkey 的字符串的长度，和输入 key 中最长的字符串长度相等。</returns>
        public int Or(string destKeyName, string[] keysName)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(destKeyName), "参数：destKeyName不能为空");
            Check.IsTure(keysName == null || keysName.Length == 0, "参数：keyName不能为空");

            var lstCmdBytes = new List<byte[]> { Commands.Bitop, Commands.Or, destKeyName.ToUtf8Bytes() };
            lstCmdBytes.AddRange(keysName.ToUtf8Bytes());

            return _redisClient.SendExpectInt(lstCmdBytes.ToArray());
        }

        /// <summary>
        /// 对一个或多个 keysName 求逻辑并，并将结果保存到 destKeyName 。
        /// </summary>
        /// <param name="destKeyName">保存的新Key</param>
        /// <param name="keysName">要计算的Key</param>
        /// <returns>保存到 destkey 的字符串的长度，和输入 key 中最长的字符串长度相等。</returns>
        public int Xor(string destKeyName, string[] keysName)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(destKeyName), "参数：destKeyName不能为空");
            Check.IsTure(keysName == null || keysName.Length == 0, "参数：keyName不能为空");

            var lstCmdBytes = new List<byte[]> { Commands.Bitop, Commands.Xor, destKeyName.ToUtf8Bytes() };
            lstCmdBytes.AddRange(keysName.ToUtf8Bytes());

            return _redisClient.SendExpectInt(lstCmdBytes.ToArray());
        }

        /// <summary>
        /// 对给定 key 求逻辑非，并将结果保存到 destkey 。
        /// </summary>
        /// <param name="destKeyName">保存的新Key</param>
        /// <param name="keysName">要计算的Key</param>
        /// <returns>保存到 destkey 的字符串的长度，和输入 key 中最长的字符串长度相等。</returns>
        public int Not(string destKeyName, string keysName)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(destKeyName), "参数：destKeyName不能为空");
            Check.IsTure(string.IsNullOrWhiteSpace(keysName), "参数：keysName不能为空");

            return _redisClient.SendExpectInt(Commands.Bitop, Commands.Not, destKeyName.ToUtf8Bytes(), keysName.ToUtf8Bytes());
        }

        /// <summary>
        /// 获取指定偏移量上的位(bit)。
        /// </summary>
        /// <param name="keyName">Key名称</param>
        /// <param name="index">要获取的位置</param>
        /// <returns>返回指定index位置的位值。</returns>
        public int Get(string keyName, int index)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(keyName), $"参数：{nameof(keyName)}不能为空");
            return _redisClient.SendExpectInt(Commands.GetBit, keyName.ToUtf8Bytes(), index.ToUtf8Bytes());
        }

        /// <summary>
        /// 对 key 所储存的字符串值，设置或清除指定偏移量上的位(bit)。位的设置或清除取决于 value 参数，可以是 0 也可以是 1 。
        /// </summary>
        /// <param name="keyName">Key名称</param>
        /// <param name="val">0或1</param>
        /// <param name="index">要获取的位置</param>
        /// <returns>指定偏移量原来储存的位。</returns>
        public int Set(string keyName, int val, int index)
        {
            return _redisClient.SendExpectInt(Commands.SetBit, keyName.ToUtf8Bytes(), index.ToUtf8Bytes(), val.ToUtf8Bytes());
        }
    }
}
