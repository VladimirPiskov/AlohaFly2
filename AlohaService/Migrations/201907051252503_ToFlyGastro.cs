namespace AlohaService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ToFlyGastro : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Dishes", "SHGastroId", c => c.Long(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Dishes", "SHGastroId");
        }
    }
}
