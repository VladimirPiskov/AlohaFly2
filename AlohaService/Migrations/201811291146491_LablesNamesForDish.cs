namespace AlohaService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LablesNamesForDish : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Dishes", "LabelRussianName", c => c.String(nullable: false, defaultValue: "RussianName"));
            AddColumn("dbo.Dishes", "LabelEnglishName", c => c.String(nullable: false, defaultValue: "EnglishName"));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Dishes", "LabelEnglishName");
            DropColumn("dbo.Dishes", "LabelRussianName");
        }
    }
}
