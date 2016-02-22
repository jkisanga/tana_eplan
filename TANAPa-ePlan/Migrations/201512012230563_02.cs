namespace TANAPa_ePlan.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _02 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DailyActivities", "HeadId", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.DailyActivities", "HeadId");
        }
    }
}
