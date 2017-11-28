#if IsWeb
using System;
using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace FS.Configs
{
    /// <summary> IP安全限制访问 </summary>
    public class WebIPSecurityConfigs : AbsConfigs<WebIPSecurityConfig> { }

    /// <summary> IP安全限制访问 </summary>
    [Serializable]
    public class WebIPSecurityConfig
    {
        /// <summary> 是否为黑名单方式（false：白名单模式、true：黑名单模式） </summary>
        public bool IsBlacklistMode = true;
        /// <summary> 被拒绝后跳转的地址 </summary>
        public string JumpUrl = "";
        /// <summary> 被拒绝后输出内容（如果设置了地址，则不会输出该内容） </summary>
        public string Content = "您没有权限查看网页内容";
        /// <summary> 被拒绝后的页面状态 </summary>
        public int StatusCode = 403;
        /// <summary> IP安全限制访问列表（支持*、-范围） </summary>
        public List<string> IPList = new List<string>();
        /// <summary> 地址（中国的门牌地址）安全限制访问列表</summary>
        public List<string> AddressList = new List<string>();
    }
}
#endif