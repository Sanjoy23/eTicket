namespace Identity.API.Models
{
    public sealed record AuthResponse(
    string Id,
    string Email,
    IList<string> Roles,
    string Token,
    DateTimeOffset ExpiresAt);
}
