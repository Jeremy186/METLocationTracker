namespace MapsProposal.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using MapsProposal.Models;
    using System.Collections.Generic;

    internal sealed class Configuration : DbMigrationsConfiguration<MapsProposal.DAL.MapsProposalContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            //ContextKey = "MapsProposal.DAL.MapsProposalContext";
        }

        protected override void Seed(MapsProposal.DAL.MapsProposalContext context)
        {
            var locations = new List<Location>
            {
                new Location { Name = "Skull Island",Latitude = 30,Longitude = 25,VegetationCover=true},
                new Location { Name = "Atlantis",    Latitude = 35,Longitude = 45,VegetationCover=false}
            };
            locations.ForEach(l => context.Locations.AddOrUpdate(p => p.Name, l));
            context.SaveChanges();
        }
    }
}
