using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dapper;
using System.Data;
using System.Data.SqlClient;

namespace An.WebSite.Controllers
{
    public class DapperController : Controller
    {
        private string sqlConnectionString = "Data Source =.; Initial Catalog = MvcDemo; User Id = sa; Password=jack116;";
        // GET: Dapper
        public ActionResult Index()
        {
            using (IDbConnection conn = new SqlConnection(sqlConnectionString))
            {
               var res= conn.Execute($"INSERT INTO [dbo].[Products]([Name] ,[cId]) VALUES (Jack, 20)");
            }
            return View();
        }
    }
}