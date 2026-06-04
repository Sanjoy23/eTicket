using System.ComponentModel.DataAnnotations;

namespace Identity.API.Models
{
    public class Address
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string Zipcode { get; set; } = string.Empty;
        [Required]
        public string AppUserId { get; set; } = string.Empty;
        public AppUser AppUser { get; set; } = null!;
    }
}
