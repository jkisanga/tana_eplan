namespace TANAPa_ePlan.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _12 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserProfile", "Status", c => c.Int(nullable: false));
            AddColumn("dbo.UserProfile", "DeactivateReason", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserProfile", "DeactivateReason");
            DropColumn("dbo.UserProfile", "Status");
        }
    }
}
