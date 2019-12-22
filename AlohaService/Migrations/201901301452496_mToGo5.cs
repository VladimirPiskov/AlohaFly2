namespace AlohaService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class mToGo5 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Payments", "FRSend", c => c.Long(nullable: false));
            AddColumn("dbo.Payments", "ToGo", c => c.Boolean(nullable: false));
            AddColumn("dbo.OrderToGoes", "Closed", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrderToGoes", "Closed");
            DropColumn("dbo.Payments", "ToGo");
            DropColumn("dbo.Payments", "FRSend");
        }
    }
}
