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

    public enum Role
    {
        User,
        Admin
    }
    public class DetailsViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Role Role { get; set; }
        public IEnumerable<Location> Locations { get; set; }
    }
}