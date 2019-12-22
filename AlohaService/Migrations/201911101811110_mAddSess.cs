namespace AlohaService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class mAddSess : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderToGoes", "LastUpdatedSession", c => c.Guid(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrderToGoes", "LastUpdatedSession");
        }
    }
}
