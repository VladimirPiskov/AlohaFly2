namespace AlohaService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class rtu : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderCustomerAddresses", "UpdatedDate", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AddColumn("dbo.OrderCustomerAddresses", "LastUpdatedSession", c => c.Guid(nullable: false));
            AddColumn("dbo.OrderCustomers", "UpdatedDate", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AddColumn("dbo.OrderCustomers", "LastUpdatedSession", c => c.Guid(nullable: false));
            AddColumn("dbo.OrderCustomerPhones", "UpdatedDate", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AddColumn("dbo.OrderCustomerPhones", "LastUpdatedSession", c => c.Guid(nullable: false));
            AlterColumn("dbo.OrderToGoes", "UpdatedDate", c => c.DateTime(precision: 7, storeType: "datetime2"));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.OrderToGoes", "UpdatedDate", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            DropColumn("dbo.OrderCustomerPhones", "LastUpdatedSession");
            DropColumn("dbo.OrderCustomerPhones", "UpdatedDate");
            DropColumn("dbo.OrderCustomers", "LastUpdatedSession");
            DropColumn("dbo.OrderCustomers", "UpdatedDate");
            DropColumn("dbo.OrderCustomerAddresses", "LastUpdatedSession");
            DropColumn("dbo.OrderCustomerAddresses", "UpdatedDate");
        }
    }
}
