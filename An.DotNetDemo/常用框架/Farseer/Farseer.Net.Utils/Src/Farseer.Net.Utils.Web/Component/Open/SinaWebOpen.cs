#if IsWeb
using System.Text.RegularExpressions;
using FS.Utils.Common;

namespace FS.Utils.Component.Open
{
    /// <summary>
    /// 新浪微博开放平台组件
    /// 如何使用：
    /// 第一步：调用静态方法  GetAuthorizeUrl 返回跳转地址。
    /// 第二步：前台进行GET方式跳转得到的URL。（将会去到微博的登陆界面。）
    /// 第三步：当用户点击登陆后，将返回，你传入的回调页面。在回调页面中，实例化本对象
    /// 第四步：根据Uid调用 GetUserInfo 获取用户的基本信息
    /// </summary>
    public class Sina
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
        /// 该access token的有效期，单位为秒。
        /// </summary>
        public string ExpiresIn { get; private set; }
        /// <summary>
        /// 在授权自动续期步骤中，获取新的Access_Token时需要提供的参数。
        /// </summary>
        public string RefreshToken { get; private set; }

        /// <summary>
        /// 当前授权用户的UID。
        /// </summary>
        public string Uid { get; private set; }

        public int Error { get; private set; }
        /// <summary>
        /// 必须传入应用的ID跟密码
        /// </summary>
        /// <param name="appID">应用ID</param>
        /// <param name="appKey">应用密码</param>
        /// <param name="authorizationCode">授权码</param>
        /// <param name="redirectUri">成功授权后的回调地址，必须是注册appid时填写的主域名下的地址，建议设置为网站首页或网站的用户中心。注意需要将url进行URLEncode。</param>
        public Sina(string appID, string appKey, string authorizationCode, string redirectUri)
        {
            try
            {
                AppID = appID;
                AppKey = appKey;
                AuthorizationCode = authorizationCode;
                redirectUri = "http://passport.ajiao.com/sinaweibo/sinaauthorizationcode/";
                // 通过授权码、请求获取令牌 
                var url = "https://api.weibo.com/oauth2/access_token";
                var param = $"?grant_type=authorization_code&client_id={AppID}&client_secret={AppKey}&code={AuthorizationCode}&redirect_uri={redirectUri}";
                var json = Net.Post(url, param);

                AccessToken = new Regex(@"(?<=""access_token"":"")[^""]*(?="")").Match(json).Value;   //  授权令牌
                ExpiresIn = new Regex(@"(?<=""expires_in"":)[^,]*(?=,)").Match(json).Value;     //  该access token的有效期，单位为秒。
                RefreshToken = new Regex(@"(?<=""remind_in"":"")[^""]*(?="")").Match(json).Value;      //  在授权自动续期步骤中，获取新的Access_Token时需要提供的参数。
                Uid = new Regex(@"(?<=""uid"":"")[^""]*(?="")").Match(json).Value;
                
            }
            catch
            {
                Error = 400;
            }
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <returns></returns>
        public string GetUserInfo()
        {
            return API("show.json", "");
        }

        /// <summary>
        /// 与开放平台通讯的通用接口
        /// </summary>
        /// <param name="apiName">接口名称</param>
        /// <param name="paras">接口的参数，不需要传入：令牌、AppID、OpenID</param>
        /// <param name="method">GET/POST</param>
        public string API(string apiName, string paras, string method = "GET")
        {
            var url = $"https://api.weibo.com/2/users/{apiName}";
            var param = $"?access_token={AccessToken}&uid={Uid}";

            if (method.ToUpper() == "GET") { return Net.Get(url + param); }

            return Net.Post(url, param);

        }

        /// <summary>
        /// 获取授权码（返回Url，手动进行Get跳转）
        /// </summary>
        public static string GetAuthorizeUrl(string appID, string callBackUrl, string state)
        {
            return $"https://api.weibo.com/oauth2/authorize?response_type=code&client_id={appID}&redirect_uri={callBackUrl}&state={state}";
        }
    }
}
#endif