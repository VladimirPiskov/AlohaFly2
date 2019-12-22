namespace AlohaService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class mUpd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TransactionTimes", "LastConfirmedTime", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AddColumn("dbo.TransactionTimes", "Confirmed", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TransactionTimes", "Confirmed");
            DropColumn("dbo.TransactionTimes", "LastConfirmedTime");
        }
    }
}
