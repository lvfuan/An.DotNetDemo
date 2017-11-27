using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using An.Common;
using System.Configuration;
using WriteErrorLogSendEmail;
using System.Net;
using System.Net.Mail;

namespace UnitTestConsole
{
    public class Program
    {
        static void Main(string[] args)
        {
            MailHelper.SendEmail("403062955@qq.com", "测试", "Hello World");
            //Log.Debug("1111111111");
            #region 自定义配置文件读取调用方法
            //var u = AppDomain.CurrentDomain.BaseDirectory.Replace("bin\\Debug\\", "") + "CustomerConfig.config";//获取当前项目的路径配置文件
            //var url = ConfigurationManager.AppSettings["url"];//读取配置文件配置的url
            //var t = ConfigHelper.GetConfig(u, "test");
            #endregion
        }
    }
}   
