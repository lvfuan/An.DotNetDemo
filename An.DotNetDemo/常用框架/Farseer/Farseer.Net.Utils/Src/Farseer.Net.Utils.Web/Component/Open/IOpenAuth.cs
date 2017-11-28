#if IsWeb
namespace FS.Utils.Component.Open
{
    /// <summary>
    /// 第三方开放平台接口
    /// </summary>
    public interface IOpenAuth
    {
        /// <summary>
        /// 应用ID
        /// </summary>
        string AppID { get; }
        /// <summary>
        /// 应用密码
        /// </summary>
        string AppKey { get; }
        /// <summary>
        /// 授权码
        /// </summary>
        string AuthorizationCode { get; }
        /// <summary>
        /// 授权令牌
        /// </summary>
        string AccessToken { get; }
        /// <summary>
        /// OpenID
        /// </summary>
        string OpenID { get; }
    }
}
#endif