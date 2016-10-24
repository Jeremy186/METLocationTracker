using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using MapsProposal.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;

[assembly: OwinStartup(typeof(MapsProposal.Startup))]

namespace MapsProposal
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            createRolesAndUsers();
        }

        private void createRolesAndUsers()
        {
            ApplicationDbContext context = new ApplicationDbContext();

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

         //   var a = roleManager.FindByName("Admin");
         //   if(a != null)
           //     roleManager.Delete(a);

           // var b = userManager.FindByEmail("jeremybennet186@gmail.com");

           // userManager.Delete(b);

           // var c = userManager.FindByName("jeremybennett186@gmail.com");
           // userManager.Delete(c);


            // In Startup iam creating first Admin Role and creating a default Admin User    
            if (!roleManager.RoleExists("Admin"))
            {

                // first we create Admin rool   
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "Admin";
                roleManager.Create(role);

                //Here we create a Admin super user who will maintain the website                  

                var user = new ApplicationUser();
                user.UserName = "jeremybennett186@gmail.com";
                user.Email = "jeremybennett186@gmail.com";

                string userPWD = "Stoneydeep1!";

                var chkUser = userManager.Create(user, userPWD);

                //Add default User to Role Admin   
                if (chkUser.Succeeded)
                {
                    var result1 = userManager.AddToRole(user.Id, "Admin");
                }
            }
        }
    }
}
