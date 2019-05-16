using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace GRM
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
        protected void Application_Error(object sender, EventArgs e)
        {

            Exception exc = Server.GetLastError();
            Server.ClearError();
            if (!Response.IsRequestBeingRedirected)
            {
                Response.Redirect("~/Home/ErrorPage/");
            }
        }

    }
}
