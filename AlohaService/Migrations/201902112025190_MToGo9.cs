namespace AlohaService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MToGo9 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderFlights", "AlohaGuidId", c => c.Guid());
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrderFlights", "AlohaGuidId");
        }
    }
}
