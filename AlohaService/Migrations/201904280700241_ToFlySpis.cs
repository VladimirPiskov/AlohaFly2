namespace AlohaService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ToFlySpis : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DishPackageFlightOrders", "Deleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.DishPackageFlightOrders", "DeletedStatus", c => c.Int(nullable: false));
            AddColumn("dbo.DishPackageFlightOrders", "SpisPaymentId", c => c.Long(nullable: false));
            AddColumn("dbo.DishPackageToGoOrders", "Deleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.DishPackageToGoOrders", "DeletedStatus", c => c.Int(nullable: false));
            AddColumn("dbo.DishPackageToGoOrders", "SpisPaymentId", c => c.Long(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.DishPackageToGoOrders", "SpisPaymentId");
            DropColumn("dbo.DishPackageToGoOrders", "DeletedStatus");
            DropColumn("dbo.DishPackageToGoOrders", "Deleted");
            DropColumn("dbo.DishPackageFlightOrders", "SpisPaymentId");
            DropColumn("dbo.DishPackageFlightOrders", "DeletedStatus");
            DropColumn("dbo.DishPackageFlightOrders", "Deleted");
        }
    }
}
