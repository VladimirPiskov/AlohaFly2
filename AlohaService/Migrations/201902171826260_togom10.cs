namespace AlohaService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class togom10 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Dishes", "SHIdNewBase", c => c.Long(nullable: false));
            AddColumn("dbo.DishKitchenGroups", "SHIdToFly", c => c.Long(nullable: false));
            AddColumn("dbo.DishKitchenGroups", "SHIdToGo", c => c.Long(nullable: false));
            AddColumn("dbo.DishKitchenGroups", "SHIdSh", c => c.Long(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.DishKitchenGroups", "SHIdSh");
            DropColumn("dbo.DishKitchenGroups", "SHIdToGo");
            DropColumn("dbo.DishKitchenGroups", "SHIdToFly");
            DropColumn("dbo.Dishes", "SHIdNewBase");
        }
    }
}
