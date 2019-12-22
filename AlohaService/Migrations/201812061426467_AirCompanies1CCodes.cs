namespace AlohaService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AirCompanies1CCodes : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AirCompanies", "Code1C", c => c.String());
            AddColumn("dbo.AirCompanies", "Name1C", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AirCompanies", "Name1C");
            DropColumn("dbo.AirCompanies", "Code1C");
        }
    }
}
