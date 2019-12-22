namespace AlohaService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MenuNameMgr : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Dishes", "MenuName", c => c.String());
            AddColumn("dbo.Dishes", "MenuEnglishName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Dishes", "MenuEnglishName");
            DropColumn("dbo.Dishes", "MenuName");
        }
    }
}
