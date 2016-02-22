namespace TANAPa_ePlan.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _002 : DbMigration
    {
        public override void Up()
        {
            AddForeignKey("dbo.DailyActivities", "DeptId", "dbo.Departments", "DeptId");
            CreateIndex("dbo.DailyActivities", "DeptId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.DailyActivities", new[] { "DeptId" });
            DropForeignKey("dbo.DailyActivities", "DeptId", "dbo.Departments");
        }
    }
}
