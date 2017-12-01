using DLL;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Autofac;
using BLL.Service;
using BLL.IService;
using Autofac.Integration.Mvc;
namespace UI
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            //Database.SetInitializer(new Initializer());
            //DependencyResolver.SetResolver(new  AutofacDependencyResolver(Register().Build()));
            RouteConfig.RegisterRoutes(RouteTable.Routes);

        }
        //private ContainerBuilder Register()
        //{
        //    var builer = new ContainerBuilder();
        //    builer.RegisterType<UserServer>().As<IUser>();
        //    builer.RegisterType<ManagementServer>().As<IManagement>();

        //    //var assemblys = System.Reflection.Assembly.Load("DLL");
        //    //builer.RegisterControllers(assemblys);
        //    //builer.RegisterFilterProvider();
        //    //builer.Build();     
        //    //var resolver = new AutofacWebApiDependencyResolver(container);
        //    //configuration.DependencyResolver = resolver;
        //    return builer;

        //}
    }
}
