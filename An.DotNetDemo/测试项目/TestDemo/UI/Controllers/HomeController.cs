using BLL.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DLL.Models;
using BLL.Service;

namespace UI.Controllers
{
    public class HomeController : Controller
    {
        private IUser _user=new UserServer();
        private IManagement _management=new ManagementServer();
        public HomeController()
        {
           
        }
        public ActionResult Index()
        {
            this._management.Add(new ManagementModel { LoginId = "Test", LoginPwd = "123", IsDelete = false, IsState = true });
            return View();
        }
    }
}