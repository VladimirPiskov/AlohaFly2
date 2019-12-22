namespace AlohaService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LogItem : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LogItems",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        MethodName = c.String(),
                        ActionName = c.String(),
                        ActionDescription = c.String(),
                        UserId = c.Long(nullable: false),
                        StateBefore = c.String(),
                        StateAfter = c.String(),
                        CreationDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.LogItems", "UserId", "dbo.Users");
            DropIndex("dbo.LogItems", new[] { "UserId" });
            DropTable("dbo.LogItems");
        }
    }
}
