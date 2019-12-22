namespace AlohaService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class mToGo12 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Dishes", "DishKitсhenGroupId", "dbo.DishKitchenGroups");
            DropForeignKey("dbo.Dishes", "DishLogicGroupId", "dbo.DishLogicGroups");
            DropIndex("dbo.Dishes", new[] { "DishLogicGroupId" });
            DropIndex("dbo.Dishes", new[] { "DishKitсhenGroupId" });
        }
        
        public override void Down()
        {
            CreateIndex("dbo.Dishes", "DishKitсhenGroupId");
            CreateIndex("dbo.Dishes", "DishLogicGroupId");
            AddForeignKey("dbo.Dishes", "DishLogicGroupId", "dbo.DishLogicGroups", "Id");
            AddForeignKey("dbo.Dishes", "DishKitсhenGroupId", "dbo.DishKitchenGroups", "Id");
        }
    }
}
