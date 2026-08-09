namespace Identity.API.Models
{
    public sealed record AuthResponse(
    string Id,
    string Email,
    IList<string> Roles,
    string Token,
    DateTimeOffset ExpiresAt,
    string RefreshToken, DateTime RefreshExpiresAt);


    public record RevokeRequest(string RefreshToken);
}
