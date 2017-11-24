using An.Common.Config;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace An.Common
{
    /// <summary>
    /// 获取自定义配置文件帮助类
    /// </summary>
    public class ConfigHelper
    {
        /// <summary>
        /// 获取配置文件信息
        /// </summary>
        /// <param name="url">路径</param>
        /// <param name="key">key值</param>
        /// <returns></returns>
        public static string GetConfig(string url,string key)
        {
            ExeConfigurationFileMap map = new ExeConfigurationFileMap();
            map.ExeConfigFilename = url;

            var configManager = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);
            if (configManager.HasFile)
            {
                CustomersSection config = (CustomersSection)configManager.GetSection("Customers");
                return config.Customers[key].Value.ToString();
            }
            return string.Empty;
        }
    }
}
