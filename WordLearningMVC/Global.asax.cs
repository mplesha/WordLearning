using BusinessLayer.Interfaces;
using log4net;
using System;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using WordLearningMVC.App_Start;
using WordLearningMVC.Controllers;
using WordLearningMVC.Infrastructure;
using WordLearningMVC.Logging;

namespace WordLearningMVC
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : HttpApplication
    {
       
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            DependencyResolver.SetResolver(new NinjectDependencyResolver());

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();

            var fileName = string.Format("Report_{0}", DateTime.Now.ToString("yyyy.MM.dd"));
            GlobalContext.Properties["LogFileName"] = fileName;
            StartUpTask task = new StartUpTask();
            task.Configure("~/log4net.config");
        
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            HttpContext ctx = HttpContext.Current;
            Exception ex = ctx.Server.GetLastError();
            ctx.Response.Clear();

            RequestContext rc = ((MvcHandler)ctx.CurrentHandler).RequestContext;
            IController controller = new HomeController();
            var context = new ControllerContext(rc, (ControllerBase)controller);
            //MainClassLogger logger = new MainClassLogger();
            ILogger logger = new MainClassLogger();
            var viewResult = new ViewResult();

            var httpException = ex as HttpException;
            if (httpException != null)
            {
                switch (httpException.GetHttpCode())
                {
                    case 404:
                        viewResult.ViewName = "Error404";
                        logger.Error("Page not found", ex);                       
                        break;

                    case 500:
                        viewResult.ViewName = "Error500";
                        logger.Error("Server Error", ex);
                        break;

                    default:
                        viewResult.ViewName = "Error";
                        logger.Error("Error", ex);
                        break;
                }
            }
            else
            {
                viewResult.ViewName = "Error";
            }

            viewResult.ViewData.Model = new HandleErrorInfo(ex, context.RouteData.GetRequiredString("controller"), context.RouteData.GetRequiredString("action"));
            viewResult.ExecuteResult(context);
            ctx.Server.ClearError();
        }
        

    }
}