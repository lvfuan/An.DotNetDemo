using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDB.Helper
{
    /// <summary>
    /// MongoDB工厂类
    /// </summary>
    public class MongoDBFactory
    {
        private string _connectionString = string.Empty;
        private string _dataBase = string.Empty;
        public MongoDBFactory()
        {
            this._connectionString = ConfigurationManager.AppSettings["MongoDBConnectionString"]; //连接字符串
            this._dataBase = ConfigurationManager.AppSettings["DataBaseName"]; //文件夹名称
        }
        /// <summary>
        /// 连接字符串
        /// </summary>
        public string connectionString { get { return this._connectionString; }}
        /// <summary>
        /// 数据库名称
        /// </summary>
        public string dataBase { get { return this._dataBase; } }
        /// <summary>
        /// 获取mongDB操作类实例
        /// </summary>
        /// <returns></returns>
        public MongoDBHelper GetInstance()
        {
            return new MongoDBHelper(connectionString, dataBase);
        }
    }
}
