using System;
using System.Net.Security;

// ReSharper disable once CheckNamespace
namespace FS.Configs
{
    /// <summary> 系统配置文件 </summary>
    public class RedisConfigs : AbsConfigs<RedisConfig>
    {
    }
    /// <summary> 系统配置文件 </summary>
    [Serializable]
    public class RedisConfig
    {
        /// <summary>
        /// 连接超时时间
        /// </summary>
        public int ConnectTimeout = -1;

        /// <summary>
        /// 命令发送时间 ( -1, None)
        /// </summary>
        public int SendTimeout = -1;

        /// <summary>
        /// 命令接收时间 ( -1, None)
        /// </summary>
        public int ReceiveTimeout = -1;

        /// <summary>
        /// 闲置超时时间 ( 240 secs)
        /// </summary>
        public int IdleTimeOutSecs = 240;

        /// <summary>
        /// RetryTimeout for auto retry of failed operations ( 3000ms)
        /// </summary>
        public int RetryTimeout = 3 * 1000;

        /// <summary>
        /// The BackOff multiplier failed Auto Retries starts from ( 10ms)
        /// </summary>
        public int BackOffMultiplier = 10;

        /// <summary>
        /// 缓冲区的最大长度
        /// </summary>
        public int BufferLength = 1450;

        /// <summary>
        /// The Byte Buffer Size for Operations to use a byte buffer pool ( 500kb)
        /// </summary>
        public int BufferPoolMaxSize = 500000;

        /// <summary>
        /// Whether Connections to Master hosts should be verified they're still master instances ( true)
        /// </summary>
        public bool VerifyMasterConnections = true;

        /// <summary>
        /// The ConnectTimeout on clients used to find the next available host ( 200ms)
        /// </summary>
        public int HostLookupTimeoutMs = 200;

        /// <summary>
        /// Skip ServerVersion Checks by specifying Min Version number, e.g: 2.8.12 => 2812, 2.9.1 => 2910
        /// </summary>
        public int? AssumeServerVersion;

        /// <summary>
        /// How long to hold deactivated clients for before disposing their connection ( 1 min)
        /// Dispose of deactivated Clients immediately with TimeSpan.Zero
        /// </summary>
        public TimeSpan DeactivatedClientsExpiry = TimeSpan.FromMinutes(1);

        /// <summary>
        /// Whether Debug Logging should log detailed Redis operations ( false)
        /// </summary>
        public bool DisableVerboseLogging = false;

        //Example at: http://msdn.microsoft.com/en-us/library/office/dd633677(v=exchg.80).aspx 
        //public LocalCertificateSelectionCallback CertificateSelectionCallback { get; set; }
        //public RemoteCertificateValidationCallback CertificateValidationCallback { get; set; }
    }
}