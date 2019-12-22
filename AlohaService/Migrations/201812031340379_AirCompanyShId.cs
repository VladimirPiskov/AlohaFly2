namespace AlohaService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AirCompanyShId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AirCompanies", "SHId", c => c.Long(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AirCompanies", "SHId");
        }
    }
}
