#if IsWeb
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.SessionState;
using FS.Configs;
using FS.Extends;
using FS.Utils.Common;

namespace FS.Utils.HttpModule
{
    /// <summary>
    ///     跳转模块的抽像基类
    /// </summary>
    /// <remarks></remarks>
    public class UrlJump : System.Web.IHttpModule, IRequiresSessionState
    {
        /// <summary>
        ///     加载事件管道
        /// </summary>
        public void Init(HttpApplication app) { app.AuthorizeRequest += UrlJump_AuthorizeRequest; }

        /// <summary>
        ///     注销
        /// </summary>
        public void Dispose() { }

        /// <summary>
        ///     执行跳转功能
        /// </summary>
        protected void UrlJump_AuthorizeRequest(object sender, EventArgs e)
        {
            var app = (HttpApplication)sender;

            var appPath = app.Context.Request.ApplicationPath;

            // 写入追踪日志消息
            app.Context.Trace.Write("Url跳转", "开始执行。");

            // 循环所有跳转规则
            foreach (var rule in WebUrlJumpConfigs.ConfigEntity.JumpRules)
            {
                // 取得规则地址
                var lookFor = "^" + ResolveUrl(appPath, rule.OriginalUrl) + "$";
                var re = new Regex(lookFor, RegexOptions.IgnoreCase);
                var url = lookFor.ToLower().StartsWith("^http://") ? app.Request.Url.AbsoluteUri : app.Request.Path;

                if (!re.IsMatch(url)) continue;

                // 取得跳转规则地址
                var sendToUrl = ResolveUrl(appPath, re.Replace(url, rule.NewUrl));
                // 写入追踪日志消息
                app.Context.Trace.Write("Url跳转", "跳转到：" + sendToUrl);
                // 跳转地址
                Jump(app.Context, sendToUrl, rule.JumpCode);
                break;
            }
            app.Context.Trace.Write("Url跳转", "结束执行");
        }

        /// <summary>
        ///     替换多个域、并且将本地路径转换成网站路径
        /// </summary>
        /// <param name="appPath">网站根目录.</param>
        /// <param name="url">规则地址</param>
        protected string ResolveUrl(string appPath, string url)
        {
            //替换多个域
            url = string.Format(url, WebGeneralConfigs.ConfigEntity.RewriterDomain.ToArray("", ";"));
            if (url.Length == 0 || url[0] != '~') { return url; }
            // 如果不是使用目录地址，则直接返回原地址
            if (url.Length == 1) { return appPath; }

            // 返回根目录地址
            return appPath + Url.ConvertPath(url[1] == '/' ? url.Substring(2) : url.Substring(1));
        }

        /// <summary>
        ///     跳转地址
        /// </summary>
        /// <param name="context">请求的上下文</param>
        /// <param name="sendToUrl">跳转的目的URL</param>
        /// <param name="code">跳转目的URL的实际物理路径</param>
        protected void Jump(HttpContext context, string sendToUrl, int code)
        {
            context.Response.Clear();
            context.Response.StatusCode = code;
            context.Response.Status = code + " Jump";
            context.Response.AddHeader("Location", sendToUrl);
            context.Response.End();
        }
    }
}
#endif