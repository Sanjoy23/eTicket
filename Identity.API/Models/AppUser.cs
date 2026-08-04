using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using static Duende.IdentityServer.Models.IdentityResources;

namespace Identity.API.Models
{

    public class AppUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? ProfilePictureUrl { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTimeOffset CreatedOn { get; set; } = DateTimeOffset.UtcNow;
        public Address Address { get; set; } = null!;
    }

}
