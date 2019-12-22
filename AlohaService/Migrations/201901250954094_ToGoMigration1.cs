namespace AlohaService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ToGoMigration1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.OrderToGoes", "CreatedById", "dbo.Users");
            DropForeignKey("dbo.OrderToGoes", "CurierId", "dbo.Curiers");
            DropForeignKey("dbo.OrderToGoes", "DeliveryPlaceId", "dbo.DeliveryPlaces");
            DropForeignKey("dbo.OrderToGoes", "OrderCustomerId", "dbo.OrderCustomers");
            DropForeignKey("dbo.OrderToGoes", "PaymentId", "dbo.Payments");
            DropForeignKey("dbo.OrderToGoes", "WhoDeliveredPersonPersonId", "dbo.DeliveryPersons");
            DropIndex("dbo.OrderToGoes", new[] { "CreatedById" });
            DropIndex("dbo.OrderToGoes", new[] { "OrderCustomerId" });
            DropIndex("dbo.OrderToGoes", new[] { "DeliveryPlaceId" });
            DropIndex("dbo.OrderToGoes", new[] { "WhoDeliveredPersonPersonId" });
            DropIndex("dbo.OrderToGoes", new[] { "CurierId" });
            DropIndex("dbo.OrderToGoes", new[] { "PaymentId" });
            CreateTable(
                "dbo.MarketingChannels",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Dishes", "IsToGo", c => c.Boolean(nullable: false));
            AddColumn("dbo.OrderToGoes", "MarketingChannelId", c => c.Long());
            DropColumn("dbo.OrderToGoes", "DeliveryPlaceId");
            DropColumn("dbo.OrderToGoes", "MarketingChannel");
        }
        
        public override void Down()
        {
            AddColumn("dbo.OrderToGoes", "MarketingChannel", c => c.Int());
            AddColumn("dbo.OrderToGoes", "DeliveryPlaceId", c => c.Long());
            DropColumn("dbo.OrderToGoes", "MarketingChannelId");
            DropColumn("dbo.Dishes", "IsToGo");
            DropTable("dbo.MarketingChannels");
            CreateIndex("dbo.OrderToGoes", "PaymentId");
            CreateIndex("dbo.OrderToGoes", "CurierId");
            CreateIndex("dbo.OrderToGoes", "WhoDeliveredPersonPersonId");
            CreateIndex("dbo.OrderToGoes", "DeliveryPlaceId");
            CreateIndex("dbo.OrderToGoes", "OrderCustomerId");
            CreateIndex("dbo.OrderToGoes", "CreatedById");
            AddForeignKey("dbo.OrderToGoes", "WhoDeliveredPersonPersonId", "dbo.DeliveryPersons", "Id");
            AddForeignKey("dbo.OrderToGoes", "PaymentId", "dbo.Payments", "Id");
            AddForeignKey("dbo.OrderToGoes", "OrderCustomerId", "dbo.OrderCustomers", "Id");
            AddForeignKey("dbo.OrderToGoes", "DeliveryPlaceId", "dbo.DeliveryPlaces", "Id");
            AddForeignKey("dbo.OrderToGoes", "CurierId", "dbo.Curiers", "Id");
            AddForeignKey("dbo.OrderToGoes", "CreatedById", "dbo.Users", "Id");
        }
    }
}
