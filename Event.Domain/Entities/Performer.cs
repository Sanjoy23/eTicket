using Event.Domain.Entities.Events;
using System.ComponentModel.DataAnnotations;

namespace Event.Domain.Entities
{
    public class Performer
    {
        [Key]
        public Guid PerformerId { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Type { get; set; } = string.Empty;

        public string? WebsiteUrl { get; set; }

        public string? ImageUrl { get; set; }

        public ICollection<EventPerformer> EventPerformers { get; set; } = [];

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
