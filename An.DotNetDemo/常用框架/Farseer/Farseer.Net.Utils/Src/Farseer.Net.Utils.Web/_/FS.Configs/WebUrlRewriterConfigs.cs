#if IsWeb
using System;
using System.Collections.Generic;
using FS.Utils;
using FS.Utils.Component;

// ReSharper disable once CheckNamespace
namespace FS.Configs
{
    /// <summary> 重写地址规则 </summary>
    public class WebUrlRewriterConfigs : AbsConfigs<WebUrlRewriterConfig> { }

    /// <summary> 重写地址规则 </summary>
    [Serializable]
    public class WebUrlRewriterConfig
    {
        /// <summary> 重写地址规则列表 </summary>
        public List<RewriterRule> RewriterRules = new List<RewriterRule>();
    }

    /// <summary> 重写地址规则 </summary>
    public class RewriterRule
    {
        /// <summary> 请求地址 </summary>
        public string Url = "";
        /// <summary> 重写地址 </summary>
        public string MapPath = "";

        /// <summary> 通过索引返回实体 </summary>
        public static implicit operator RewriterRule(int index)
        {
            return WebUrlRewriterConfigs.ConfigEntity.RewriterRules.Count <= index ? null : WebUrlRewriterConfigs.ConfigEntity.RewriterRules[index];
        }
    }
}
#endif