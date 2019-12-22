namespace AlohaService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DishNeedPrintedInmenuAndOrderShSent : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Dishes", "NeedPrintInMenu", c => c.Boolean(nullable: false, defaultValue: true));
            AddColumn("dbo.OrderFlights", "IsSHSent", c => c.Boolean(nullable: false, defaultValue: false));
            AddColumn("dbo.OrderToGoes", "IsSHSent", c => c.Boolean(nullable: false, defaultValue: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrderToGoes", "IsSHSent");
            DropColumn("dbo.OrderFlights", "IsSHSent");
            DropColumn("dbo.Dishes", "NeedPrintInMenu");
        }
    }
}
