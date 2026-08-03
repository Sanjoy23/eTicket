using Identity.API.Models;

namespace Identity.API.Interfaces
{
    public interface ITokenService
    {
        (string Token, DateTime ExpiresAt) CreateToken(AppUser user, IEnumerable<string> roles);
    }
}
