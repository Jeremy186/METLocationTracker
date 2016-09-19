namespace MapsProposal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removedtestcolumn : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Location", "Test");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Location", "Test", c => c.Boolean(nullable: false));
        }
    }
}
