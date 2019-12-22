namespace AlohaService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddEnglishGroupNames : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DishKitchenGroups", "EnglishName", c => c.String());
            AddColumn("dbo.DishLogicGroups", "EnglishName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.DishLogicGroups", "EnglishName");
            DropColumn("dbo.DishKitchenGroups", "EnglishName");
        }
    }
}
