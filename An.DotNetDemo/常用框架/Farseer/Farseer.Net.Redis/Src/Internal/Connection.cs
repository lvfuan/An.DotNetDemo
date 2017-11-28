using System.Net.Sockets;
using FS.Configs;
using FS.Extends;

namespace FS.Redis.Internal
{
    /// <summary>
    ///     上下文数据库连接信息
    /// </summary>
    internal class Connection
    {
        /// <summary>
        ///     数据库IP地址
        /// </summary>
        public string IP { get; set; }
        /// <summary>
        ///     数据库端口
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        ///     数据库密码
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        ///     数据库密码
        /// </summary>
        public long Db { get; set; }

        /// <summary>
        ///     上下文初始化器（只赋值，不初始化，有可能被重复创建两次）
        /// </summary>
        /// <param name="ip">数据库IP地址</param>
        /// <param name="port">数据库端口</param>
        /// <param name="password">数据库密码（没有可不填）</param>
        /// <param name="db">库</param>
        public Connection(string ip, int port, string password, long db)
        {
            this.IP = ip ?? DefaultConfig.IP;
            this.Port = port > 0 ? port : DefaultConfig.Port;
            this.Password = password;
            Db = db;
        }
    }
}