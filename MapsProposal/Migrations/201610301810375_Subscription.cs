namespace MapsProposal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Subscription : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Subscription",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Sattelite = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            AddColumn("dbo.Location", "SubscriptionId", c => c.Int());
            CreateIndex("dbo.Location", "SubscriptionId");
            AddForeignKey("dbo.Location", "SubscriptionId", "dbo.Subscription", "ID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Location", "SubscriptionId", "dbo.Subscription");
            DropIndex("dbo.Location", new[] { "SubscriptionId" });
            DropColumn("dbo.Location", "SubscriptionId");
            DropTable("dbo.Subscription");
        }
    }
}
