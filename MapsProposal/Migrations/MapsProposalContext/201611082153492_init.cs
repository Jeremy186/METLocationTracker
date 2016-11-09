namespace MapsProposal.Migrations.MapsProposalContext
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Location",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        UserId = c.Guid(nullable: false),
                        VegetationCover = c.Boolean(nullable: false),
                        LandUse = c.Boolean(nullable: false),
                        Forestry = c.Boolean(nullable: false),
                        Influx = c.Boolean(nullable: false),
                        SocialInfrastructure = c.Boolean(nullable: false),
                        SettlementSize = c.Boolean(nullable: false),
                        Water = c.Boolean(nullable: false),
                        Area = c.Boolean(nullable: false),
                        Height = c.Boolean(nullable: false),
                        LocationType = c.Int(nullable: false),
                        Latitude = c.Double(),
                        Longitude = c.Double(),
                        NorthEastLatitude = c.Double(),
                        NorthEastLongitude = c.Double(),
                        SouthWestLatitude = c.Double(),
                        SouthWestLongitude = c.Double(),
                        RectangleArea = c.Double(),
                        SubscriptionId = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Subscription", t => t.SubscriptionId)
                .Index(t => t.SubscriptionId);
            
            CreateTable(
                "dbo.Subscription",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Sattelite = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Location", "SubscriptionId", "dbo.Subscription");
            DropIndex("dbo.Location", new[] { "SubscriptionId" });
            DropTable("dbo.Subscription");
            DropTable("dbo.Location");
        }
    }
}
