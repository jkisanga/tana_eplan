using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;


public class AccessAttribute : AuthorizeAttribute
{
    public override void OnAuthorization(AuthorizationContext filterContext)
    {
         base.OnAuthorization(filterContext);

         if (filterContext.Result is HttpUnauthorizedResult)
         {            
             filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary { { "action", "Index" }, { "controller", "Unauthorised" }, { "area", null } });
         }
    }
}
