#if IsWeb
using FS.Utils.Common;

namespace FS.Utils.Component.Open
{
    /// <summary>
    /// 微信开放平台组件
    /// 如何使用：
    /// 第一步：调用静态方法  GetAuthorizeUrl 返回跳转地址。
    /// 第二步：前台进行GET方式跳转得到的URL。（将会去到QQ的登陆界面。）
    /// 第三步：当用户点击登陆后，将返回，你传入的回调页面。在回调页面中，实例化本对象
    /// 第四步：调用 GetOpenID 获取OpenID。调用 GetUserInfo 获取用户的基本信息
    /// </summary>
    public class WX : IOpenAuth
    {
        /// <summary>
        /// 应用ID
        /// </summary>
        public string AppID { get; private set; }
        /// <summary>
        /// 应用密码
        /// </summary>
        public string AppKey { get; private set; }
        /// <summary>
        /// 授权码
        /// </summary>
        public string AuthorizationCode { get; private set; }
        /// <summary>
        /// 授权令牌
        /// </summary>
        public string AccessToken { get; private set; }

        /// <summary>
        /// OpenID
        /// </summary>
        public string OpenID { get; private set; }
        /// <summary>
        /// 该access token的有效期，单位为秒。
        /// </summary>
        public string ExpiresIn { get; private set; }
        /// <summary>
        /// 在授权自动续期步骤中，获取新的Access_Token时需要提供的参数。
        /// </summary>
        public string RefreshToken { get; private set; }
        /// <summary>
        /// 用户授权的作用域，使用逗号（,）分隔
        /// </summary>
        public string Scope { get; private set; }
        /// <summary>
        /// 只有在用户将公众号绑定到微信开放平台帐号后，才会出现该字段。
        /// </summary>
        public string Unionid { get; private set; }

        /// <summary>
        /// 必须传入应用的ID跟密码
        /// </summary>
        /// <param name="appID">应用ID</param>
        /// <param name="appKey">应用密码</param>
        /// <param name="authorizationCode">授权码</param>
        public WX(string appID, string appKey, string authorizationCode)
        {
           
                AppID = appID;
                AppKey = appKey;
                AuthorizationCode = authorizationCode;

                // 通过授权码、请求获取令牌 
                var url = string.Format("https://api.weixin.qq.com/sns/oauth2/access_token?appid={0}&secret={1}&code={2}&grant_type=authorization_code", AppID, AppKey, AuthorizationCode);

                var json = Net.Get(url);

                // 封装参数
                AccessToken = Url.GetParm(json, "access_token");   //  授权令牌
                ExpiresIn = Url.GetParm(json, "expires_in");     //  该access token的有效期，单位为秒。
                RefreshToken = Url.GetParm(json, "refresh_token");   //  在授权自动续期步骤中，获取新的Access_Token时需要提供的参数。
                OpenID = Url.GetParm(json, "openid");
                Scope = Url.GetParm(json, "scope");
                Unionid = Url.GetParm(json, "unionid");
            
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <returns></returns>
        public string GetUserInfo()
        {
            return API("userinfo", "");
        }

        /// <summary>
        /// 与开放平台通讯的通用接口
        /// </summary>
        /// <param name="apiName">接口名称</param>
        /// <param name="paras">接口的参数，不需要传入：令牌、AppID、OpenID</param>
        /// <param name="method">GET/POST</param>
        public string API(string apiName, string paras, string method = "GET")
        {
            var url = string.Format("https://api.weixin.qq.com/sns/{0}", apiName);
            var param = string.Format("?access_token={0}&openid={1}", AccessToken, OpenID, paras);
            if (method.ToUpper() == "GET") { return Net.Get(url + param); }

            return Net.Post(url, param);
        }

        /// <summary>
        /// 获取授权码（返回Url，手动进行Get跳转）
        /// </summary>
        public static string GetAuthorizeUrl(string appID, string callBackUrl, string state)
        {
            return string.Format("https://open.weixin.qq.com/connect/qrconnect?appid={0}&redirect_uri={1}&response_type=code&scope=snsapi_login&state={2}#wechat_redirect", appID, callBackUrl, state);
        }
    }
}
#endif