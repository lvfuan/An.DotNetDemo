#if IsWeb
namespace FS.Utils.Component.Routing
{
    /// <summary>
    ///     路由数据
    /// </summary>
    public class DomainData
    {
        /// <summary>
        /// </summary>
        public string Protocol { get; set; }

        /// <summary>
        /// </summary>
        public string HostName { get; set; }

        /// <summary>
        /// </summary>
        public string Fragment { get; set; }
    }
}
#endif