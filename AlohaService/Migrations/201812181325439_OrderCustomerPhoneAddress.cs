namespace AlohaService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OrderCustomerPhoneAddress : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.OrderCustomerAddresses",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Address = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        IsPrimary = c.Boolean(nullable: false),
                        OrderCustomerId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.OrderCustomers", t => t.OrderCustomerId, cascadeDelete: true)
                .Index(t => t.OrderCustomerId);
            
            CreateTable(
                "dbo.OrderCustomerPhones",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Phone = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        IsPrimary = c.Boolean(nullable: false),
                        OrderCustomerId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.OrderCustomers", t => t.OrderCustomerId, cascadeDelete: true)
                .Index(t => t.OrderCustomerId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OrderCustomerPhones", "OrderCustomerId", "dbo.OrderCustomers");
            DropForeignKey("dbo.OrderCustomerAddresses", "OrderCustomerId", "dbo.OrderCustomers");
            DropIndex("dbo.OrderCustomerPhones", new[] { "OrderCustomerId" });
            DropIndex("dbo.OrderCustomerAddresses", new[] { "OrderCustomerId" });
            DropTable("dbo.OrderCustomerPhones");
            DropTable("dbo.OrderCustomerAddresses");
        }
    }
}
