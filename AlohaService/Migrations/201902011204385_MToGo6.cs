namespace AlohaService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MToGo6 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PaymentGroups",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Code = c.Int(nullable: false),
                        Name = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        Sale = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Payments", "PaymentGroupId", c => c.Long(nullable: false));
            CreateIndex("dbo.Payments", "PaymentGroupId");
            //AddForeignKey("dbo.Payments", "PaymentGroupId", "dbo.PaymentGroups", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Payments", "PaymentGroupId", "dbo.PaymentGroups");
            //DropIndex("dbo.Payments", new[] { "PaymentGroupId" });
            DropColumn("dbo.Payments", "PaymentGroupId");
            DropTable("dbo.PaymentGroups");
        }
    }
}
