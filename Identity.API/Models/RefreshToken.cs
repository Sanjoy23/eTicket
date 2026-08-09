namespace Identity.API.Models
{
    public class RefreshToken
    {
        public Guid Id { get; set; }
        public string? Token { get; set; }
        public string? UserId { get; set; }
        public DateTime Created { get; set; }
        public DateTime Expires { get; set; }
        public DateTime? Revoked { get; set; }

        public string? ReplacedByToken { get; set; }
        public bool IsActive => Revoked is null && DateTime.UtcNow < Expires;
    }
}
