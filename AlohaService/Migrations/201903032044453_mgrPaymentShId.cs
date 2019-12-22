namespace AlohaService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class mgrPaymentShId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Payments", "SHId", c => c.Long(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Payments", "SHId");
        }
    }
}
