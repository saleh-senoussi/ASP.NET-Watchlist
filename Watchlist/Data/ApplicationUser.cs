using System;
using System.Collections.Generic;
//using Microsoft.AspNetCore.Identity.IdentityUser;
namespace Watchlist.Data
{
    public class ApplicationUser : Microsoft.AspNetCore.Identity.IdentityUser
    {
        public ApplicationUser() : base()
        {
            this.Watchlist = new HashSet<UserMovie>();
        }

        public string FirstName { get; set; }
        public virtual ICollection<UserMovie> Watchlist { get; set; }
    }
}
