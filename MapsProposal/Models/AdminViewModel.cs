using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace MapsProposal.Models
{
    public class AdminViewModel
    {
        public IEnumerable<ApplicationUser> Users { get; set; }
    }

    public class DetailsViewModel
    {
        public ApplicationUser User { get; set; }
        public IEnumerable<Location> Locations { get; set; }
    }
}