using An.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace An.MVCDemo.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            var url= AppDomain.CurrentDomain.BaseDirectory.Replace("bin\\Debug\\", "")+ "test.config";
            var t = ConfigHelper.GetConfig(url, "test");
            return View();
        }
    }
}