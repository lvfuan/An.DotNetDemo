using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WriteErrorLogSendEmail;
namespace An.MVCDemo.Controllers
{
    public class LogController : Controller
    {
        // GET: Log
        public ActionResult Index()
        {
            Log.Debug("1111111111");
            return View();
        }
    }
}