using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using static Duende.IdentityServer.Models.IdentityResources;

namespace Identity.API.Models
{

    public class AppUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public Address Address { get; set; } = null!;
    }

}
