namespace AlohaService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ToGoMigration3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderToGoes", "CommentKitchen", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrderToGoes", "CommentKitchen");
        }
    }
}
