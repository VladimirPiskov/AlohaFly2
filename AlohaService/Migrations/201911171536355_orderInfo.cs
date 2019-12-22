namespace AlohaService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class orderInfo : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.OrderCustomerInfoes",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        OrderCustomerId = c.Long(nullable: false),
                        OrderCount = c.Int(nullable: false),
                        MoneyCount = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.OrderCustomerInfoes");
        }
    }
}
