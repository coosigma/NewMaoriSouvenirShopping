using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace MaoriSouvenirShopping.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string FullName
        {
            get { return FirstName + " " + LastName; }
        }
        public ICollection<Order> Orders { get; set; }
        public bool Enabled { get; set; }
        public string Address { get; set; }
    }
}
