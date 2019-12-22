namespace AlohaService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ToGoMigration2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderToGoes", "DriverId", c => c.Long());
            AddColumn("dbo.OrderToGoes", "DiscountPercent", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.OrderCustomers", "DiscountPercent", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("dbo.OrderToGoes", "WhoDeliveredPersonPersonId");
            DropColumn("dbo.OrderToGoes", "CurierId");
            DropColumn("dbo.OrderToGoes", "ExtraCharge");
            DropColumn("dbo.OrderToGoes", "DiscountSumm");
        }
        
        public override void Down()
        {
            AddColumn("dbo.OrderToGoes", "DiscountSumm", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.OrderToGoes", "ExtraCharge", c => c.String());
            AddColumn("dbo.OrderToGoes", "CurierId", c => c.Long());
            AddColumn("dbo.OrderToGoes", "WhoDeliveredPersonPersonId", c => c.Long());
            DropColumn("dbo.OrderCustomers", "DiscountPercent");
            DropColumn("dbo.OrderToGoes", "DiscountPercent");
            DropColumn("dbo.OrderToGoes", "DriverId");
        }
    }
}
