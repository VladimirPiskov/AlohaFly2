namespace AlohaService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MUpdates1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TransactionTimes",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Transaction = c.Guid(nullable: false),
                        LastUpdatedTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.OrderToGoes", "UpdatedDate", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrderToGoes", "UpdatedDate");
            DropTable("dbo.TransactionTimes");
        }
    }
}
