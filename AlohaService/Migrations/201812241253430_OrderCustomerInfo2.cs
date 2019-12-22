namespace AlohaService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OrderCustomerInfo2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderCustomers", "SecondName", c => c.String());
            AddColumn("dbo.OrderCustomers", "MiddleName", c => c.String());
            AddColumn("dbo.OrderCustomers", "Email", c => c.String());
            AddColumn("dbo.OrderCustomers", "Comments", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrderCustomers", "Comments");
            DropColumn("dbo.OrderCustomers", "Email");
            DropColumn("dbo.OrderCustomers", "MiddleName");
            DropColumn("dbo.OrderCustomers", "SecondName");
        }
    }
}
