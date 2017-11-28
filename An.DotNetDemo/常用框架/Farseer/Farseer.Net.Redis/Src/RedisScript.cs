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
    /// Script（脚本）
    /// </summary>
    public class RedisScript
    {
        /// <summary>
        /// Redis客户端
        /// </summary>
        private readonly RedisClient _redisClient;
        private RedisScript() { }
        /// <summary>
        /// Script（脚本）
        /// </summary>
        /// <param name="redisClient">Redis客户端</param>
        internal RedisScript(RedisClient redisClient)
        {
            _redisClient = redisClient;
        }

        /// <summary>
        /// 通过内置的 Lua 解释器，可以使用 EVAL 命令对 Lua 脚本进行求值，并加入到缓存中。
        /// </summary>
        /// <param name="script">一段 Lua 5.1 脚本程序，它会被运行在 Redis 服务器上下文中</param>
        /// <param name="keyNames">使用KEYS[index]访问，索引由1开始计算</param>
        /// <param name="args">附加参数，使用ARGV[index]访问，索引由1开始计算</param>
        public List<string> ExecuteScript(string script, string[] keyNames, string[] args)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(script), $"参数：{nameof(script)}不能为空");
            var lstCmdBytes = new List<byte[]> { Commands.Eval, ("\"" + script + "\"").ToUtf8Bytes(), keyNames?.Length.ToUtf8Bytes() };
            if (keyNames?.Length > 0) { lstCmdBytes.AddRange(keyNames.ToUtf8Bytes()); }
            if (args?.Length > 0) { lstCmdBytes.AddRange(args.ToUtf8Bytes()); }

            return _redisClient.SendExpectMultiData(lstCmdBytes.ToArray()).ToListByUtf8<string>();
        }

        /// <summary>
        /// 根据给定的 sha1 校验码，对缓存在服务器中的脚本进行求值。
        /// </summary>
        /// <param name="sha1"> script 的 SHA1 校验和</param>
        /// <param name="keyNames">使用KEYS[index]访问，索引由1开始计算</param>
        /// <param name="args">附加参数，使用ARGV[index]访问，索引由1开始计算</param>
        public string ExecuteScriptCache(string sha1, string[] keyNames, string[] args)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(sha1), $"参数：{nameof(sha1)}不能为空");
            var lstCmdBytes = new List<byte[]> { Commands.EvalSha, sha1.ToUtf8Bytes(), keyNames?.Length.ToUtf8Bytes() };
            if (keyNames?.Length > 0) { lstCmdBytes.AddRange(keyNames.ToUtf8Bytes()); }
            if (args?.Length > 0) { lstCmdBytes.AddRange(args.ToUtf8Bytes()); }

            return _redisClient.SendExpectString(lstCmdBytes.ToArray());
        }

        /// <summary>
        /// 将脚本 script 添加到脚本缓存中，但并不立即执行这个脚本。
        /// </summary>
        /// <param name="script">一段 Lua 5.1 脚本程序</param>
        /// <returns>给定 script 的 SHA1 校验和</returns>
        public string AddCache(string script)
        {
            Check.IsTure(string.IsNullOrWhiteSpace(script), $"参数：{nameof(script)}不能为空");
            var lstCmdBytes = new List<byte[]>();
            lstCmdBytes.AddRange(Commands.ScriptLoad);
            lstCmdBytes.Add(("\"" + script + "\"").ToUtf8Bytes());

            return _redisClient.SendExpectString(lstCmdBytes.ToArray());
        }

        /// <summary>
        /// 判断scripts是否存在缓存中
        /// </summary>
        /// <param name="scriptShas">script SHAS值</param>
        /// <returns>给定 script 的 SHA1 校验和</returns>
        public bool Exists(params string[] scriptShas)
        {
            Check.IsTure(scriptShas == null || scriptShas.Length == 0, $"参数：{nameof(scriptShas)}必须至少有1个值");
            var lstCmdBytes = new List<byte[]>();
            lstCmdBytes.AddRange(Commands.ScriptExists);
            lstCmdBytes.AddRange(scriptShas.ToUtf8Bytes());

            return _redisClient.SendExpectInt(lstCmdBytes.ToArray()) == 1;
        }

        /// <summary>
        /// 清除所有 Lua 脚本缓存。
        /// </summary>
        public void Clear()
        {
            _redisClient.SendExpectString(Commands.ScriptFlush);
        }

        /// <summary>
        /// 杀死当前正在运行的 Lua 脚本，当且仅当这个脚本没有执行过任何写操作时，这个命令才生效
        /// </summary>
        /// <returns>执行成功返回 OK ，否则返回一个错误。</returns>
        public string Exit()
        {
            return _redisClient.SendExpectString(Commands.ScriptKill);
        }
    }
}
