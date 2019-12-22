namespace AlohaService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MToGo8 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Discounts", "ToGo", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Discounts", "ToGo");
        }
    }
}
