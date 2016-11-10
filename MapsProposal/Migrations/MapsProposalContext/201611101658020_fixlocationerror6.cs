namespace MapsProposal.Migrations.MapsProposalContext
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixlocationerror6 : DbMigration
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
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Subscription");
        }
    }
}
