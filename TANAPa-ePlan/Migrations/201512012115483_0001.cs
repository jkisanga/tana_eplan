namespace TANAPa_ePlan.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _0001 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AdhocTasks", "Attachment2", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AdhocTasks", "Attachment2");
        }
    }
}
