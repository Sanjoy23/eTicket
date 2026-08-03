using Microsoft.AspNetCore.Identity;

namespace Identity.API.Models
{
    public class UserRole : IdentityRole
    {
        public string? Description { get; set; }

        public UserRole(string name) : base(name)
        {
            Description = string.Empty;
        }

        public UserRole(string name, string description)
        {
            Description = description;
        }

    }
}
