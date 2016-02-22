namespace TANAPa_ePlan.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using System.Web.Security;
    using WebMatrix.WebData;

    internal sealed class Configuration : DbMigrationsConfiguration<TANAPa_ePlan.Models.ContextModel>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(TANAPa_ePlan.Models.ContextModel context)
        {
        }
        private void SeedMembership()
        {
            if (!WebSecurity.Initialized)
            {
                WebSecurity.InitializeDatabaseConnection("ContextModel",
                    "UserProfile", "UserId", "UserName", autoCreateTables: true);
            }

            var roles = (SimpleRoleProvider)Roles.Provider;
            var membership = (SimpleMembershipProvider)Membership.Provider;

            if (!roles.RoleExists("admin"))
            {
                roles.CreateRole("admin");
            }
            if (membership.GetUser("admin", false) == null)
            {
                membership.CreateUserAndAccount("admin", "admin");
            }
            if (!roles.GetRolesForUser("admin").Contains("admin"))
            {
                roles.AddUsersToRoles(new[] { "admin" }, new[] { "admin" });
            }
        }
    }
}
