namespace AlohaService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MToGo11 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Dishes", "IsShar", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Dishes", "IsShar");
        }
    }
}
