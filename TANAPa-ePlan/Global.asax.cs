using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using TANAPa_ePlan.Migrations;
using WebMatrix.WebData;

namespace TANAPa_ePlan
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            var migrator = new DbMigrator(new Configuration());
            migrator.Update();

            if (!WebSecurity.Initialized)
            {
                WebSecurity.InitializeDatabaseConnection("ContextModel", "UserProfile", "UserId", "UserName", autoCreateTables: true);
            }


            var roles = (SimpleRoleProvider)Roles.Provider;

            var membership = (SimpleMembershipProvider)System.Web.Security.Membership.Provider;

            //Insert Roles
            if (!roles.RoleExists("Admin"))
            {
                roles.CreateRole("Admin");
            }

            if (!roles.RoleExists("User"))
            {
                roles.CreateRole("User");
            }

            if (!roles.RoleExists("Manager"))
            {
                roles.CreateRole("Manager");
            }

           

            //Create User Accounts
            if (membership.GetUser("Admin", false) == null)
            {
                WebSecurity.CreateUserAndAccount("Admin", "Admin", new { DeptId = 11});
            }
            if (!roles.GetRolesForUser("Admin").Contains("Admin"))
            {
                roles.AddUsersToRoles(new[] { "Admin" }, new[] { "Admin" });
            }
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();
        }

        //protected void Application_Error(object sender, EventArgs e)
        //{
        //    Exception ex = Server.GetLastError();

        //    if (ex.Message.Contains("No user found was found that has the name"))
        //    {
        //        Response.Redirect("Account/Login");
        //    }
        //}
    }
}