using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Data;
using System.Data.SqlClient;
using An.WebSite.Models;

namespace TestConsole
{
    class Program
    {
        private static string sqlConnectionString = "Data Source =.; Initial Catalog = MvcDemo; User Id = sa; Password=jack116;";
        static void Main(string[] args)
        {
            using (IDbConnection conn = new SqlConnection(sqlConnectionString))
            {
                var res = conn.Execute($"INSERT INTO [dbo].[Products]([Name] ,[cId]) VALUES ('Jack', 20)");
            }
            using (IDbConnection conn = new SqlConnection(sqlConnectionString))
            {
                var res = conn.Query<Products>($"SELECT * FROM [dbo].[Products]");
            }
        }
    }
}
