#if IsWeb
using System;
using System.Web;
using System.Web.SessionState;
using FS.Configs;
using FS.Extends;
using FS.Utils.Common;
using FS.Utils.Component;
using System.Linq;

namespace FS.Utils.HttpModule
{
    /// <summary>
    ///     跳转模块的抽像基类
    /// </summary>
    /// <remarks></remarks>
    public class IPSecurity : System.Web.IHttpModule, IRequiresSessionState
    {
        /// <summary>
        ///     加载事件管道
        /// </summary>
        public void Init(HttpApplication app) { app.AuthorizeRequest += IPSecurity_AuthorizeRequest; }

        /// <summary>
        ///     注销
        /// </summary>
        public void Dispose() { }

        /// <summary>
        ///     执行跳转功能
        /// </summary>
        protected void IPSecurity_AuthorizeRequest(object sender, EventArgs e)
        {
            var app = (HttpApplication)sender;
            var clientIP = Req.GetIP();
            var config = WebIPSecurityConfigs.ConfigEntity;

            //app.Response.Write($"<!--{location.Area}|| {location.Province}|| {location.City}|| {location.Address}-->");

            // 循环所有跳转规则
            if (config.IPList.Select(ip => IpAdress.IsContains(clientIP, ip)).Any(result => config.IsBlacklistMode == result)) { Jump(app.Context); return; }

            // 中文地址的方式
            if (config.AddressList.Count <= 0) return;
            var location = IpAdress.GetLocation(clientIP);
            if (config.IsBlacklistMode != config.AddressList.Any(o => location.Area.Contains(o)|| location.Province.Contains(o)|| location.City.Contains(o)|| location.Address.Contains(o))) return;
            Jump(app.Context);
        }

        /// <summary>
        ///     跳转地址
        /// </summary>
        /// <param name="context">请求的上下文</param>
        protected void Jump(HttpContext context)
        {
            context.Response.Clear();
            context.Response.StatusCode = WebIPSecurityConfigs.ConfigEntity.StatusCode;
            context.Response.Status = WebIPSecurityConfigs.ConfigEntity.StatusCode + " Jump";

            // 跳转方式
            if (!string.IsNullOrWhiteSpace(WebIPSecurityConfigs.ConfigEntity.JumpUrl)) { context.Response.AddHeader("Location", WebIPSecurityConfigs.ConfigEntity.JumpUrl); }
            else
            {
                // 文字方式
                context.Response.Write(WebIPSecurityConfigs.ConfigEntity.Content);
                context.Response.End();
            }
        }
    }
}
#endif