namespace AlohaService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ToGoMigration4 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderToGoes", "AddressId", c => c.Long(nullable: false));
            AddColumn("dbo.OrderToGoes", "Summ", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.OrderToGoes", "DeliveryPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.OrderToGoes", "NeedPrintFR", c => c.Boolean(nullable: false));
            AddColumn("dbo.OrderToGoes", "NeedPrintPrecheck", c => c.Boolean(nullable: false));
            AddColumn("dbo.OrderToGoes", "FRPrinted", c => c.Boolean(nullable: false));
            AddColumn("dbo.OrderToGoes", "PreCheckPrinted", c => c.Boolean(nullable: false));
            AddColumn("dbo.OrderCustomerAddresses", "OldId", c => c.Long(nullable: false));
            DropColumn("dbo.OrderToGoes", "Code");
        }
        
        public override void Down()
        {
            AddColumn("dbo.OrderToGoes", "Code", c => c.Int());
            DropColumn("dbo.OrderCustomerAddresses", "OldId");
            DropColumn("dbo.OrderToGoes", "PreCheckPrinted");
            DropColumn("dbo.OrderToGoes", "FRPrinted");
            DropColumn("dbo.OrderToGoes", "NeedPrintPrecheck");
            DropColumn("dbo.OrderToGoes", "NeedPrintFR");
            DropColumn("dbo.OrderToGoes", "DeliveryPrice");
            DropColumn("dbo.OrderToGoes", "Summ");
            DropColumn("dbo.OrderToGoes", "AddressId");
        }
    }
}
