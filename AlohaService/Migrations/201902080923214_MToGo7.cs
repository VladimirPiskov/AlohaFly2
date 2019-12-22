namespace AlohaService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MToGo7 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.OrderToGoes", "AddressId", c => c.Long());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.OrderToGoes", "AddressId", c => c.Long(nullable: false));
        }
    }
}
