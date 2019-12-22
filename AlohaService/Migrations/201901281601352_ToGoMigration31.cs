namespace AlohaService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ToGoMigration31 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderToGoes", "OldId", c => c.Long(nullable: false));
            AddColumn("dbo.OrderCustomerAddresses", "MapUrl", c => c.String());
            AddColumn("dbo.OrderCustomerAddresses", "SubWay", c => c.String());
            AddColumn("dbo.OrderCustomerAddresses", "Comment", c => c.String());
            AddColumn("dbo.OrderCustomerAddresses", "ZoneId", c => c.Long(nullable: false));
            AddColumn("dbo.OrderCustomers", "OldId", c => c.Long(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrderCustomers", "OldId");
            DropColumn("dbo.OrderCustomerAddresses", "ZoneId");
            DropColumn("dbo.OrderCustomerAddresses", "Comment");
            DropColumn("dbo.OrderCustomerAddresses", "SubWay");
            DropColumn("dbo.OrderCustomerAddresses", "MapUrl");
            DropColumn("dbo.OrderToGoes", "OldId");
        }
    }
}
