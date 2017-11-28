#if IsWeb
using System;
using System.Collections.Generic;
using FS.Utils;
using FS.Utils.Component;

// ReSharper disable once CheckNamespace
namespace FS.Configs
{
    /// <summary> 地址跳转 </summary>
    public class WebUrlJumpConfigs : AbsConfigs<WebUrlJumpConfig> { }

    /// <summary> 地址跳转规则 </summary>
    [Serializable]
    public class WebUrlJumpConfig
    {
        /// <summary> 地址跳转规则列表 </summary>
        public List<JumpRule> JumpRules = new List<JumpRule>();
    }

    /// <summary> 地址跳转规则 </summary>
    public class JumpRule
    {
        /// <summary> 原地址 </summary>
        public string OriginalUrl = "";
        /// <summary> 跳转地址 </summary>
        public string NewUrl = "";
        /// <summary> 跳转代码 </summary>
        public int JumpCode = 301;

        /// <summary> 通过索引返回实体 </summary>
        public static implicit operator JumpRule(int index)
        {
            return WebUrlJumpConfigs.ConfigEntity.JumpRules.Count <= index ? null : WebUrlJumpConfigs.ConfigEntity.JumpRules[index];
        }
    }
}
#endif