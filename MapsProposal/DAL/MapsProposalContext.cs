using MapsProposal.Models;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace MapsProposal.DAL
{
    public class MapsProposalContext : DbContext
    {
        public MapsProposalContext() : base ("MapsProposalContext")
        {
        }

        public DbSet<Location> Locations { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

        }
    }
}