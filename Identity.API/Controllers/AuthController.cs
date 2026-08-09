using Identity.API.Data;
using Identity.API.DTOs;
using Identity.API.Extensions;
using Identity.API.Interfaces;
using Identity.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Identity.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(UserManager<AppUser> userManager,
                                SignInManager<AppUser> signInManager,
                                IOptions<JwtSettings> jwtSettings,
                                ApplicationDbContext dbContext,
                                ITokenService tokenService) : ControllerBase
    {

        private readonly ITokenService _tokenService = tokenService;
        private readonly UserManager<AppUser> _userManager = userManager;
        private readonly SignInManager<AppUser> _signInManager = signInManager;

        private readonly ApplicationDbContext _dbContext = dbContext;
        private readonly IOptions<JwtSettings> _jwtSettings = jwtSettings;

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            var user = await _userManager.FindByEmailFromClaimsPrinciple(HttpContext.User);
            if (user is null) return NotFound();
            var roles = await _userManager.GetRolesAsync(user);
            var (token, expireAt) = _tokenService.CreateToken(user, roles);
            return new UserDto
            {
                Email = user.Email!,
                Token = token,
                FirstName = user.FirstName,
                LastName = user.LastName,
            };
        }

        [HttpGet("emailexists")]
        public async Task<ActionResult<bool>> CheckEmailExistsAsync([FromQuery] string email)
        {
            return await _userManager.FindByEmailAsync(email) != null;
        }

        [HttpPost("login")]
        public async Task<IResult> Login(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
                return Results.Unauthorized();

            var roles = await _userManager.GetRolesAsync(user);
            var (accessToken, accessExpiresAt) = _tokenService.CreateToken(user, roles);

            var refreshToken = _tokenService.CreateRefreshToken();
            var refreshExpireAt = DateTime.UtcNow.AddDays(_jwtSettings.Value.RefreshTokenDays);

            _dbContext.RefreshTokens.Add(new RefreshToken
            {
                Token = refreshToken,
                UserId = user.Id,
                Created = DateTime.UtcNow,
                Expires = refreshExpireAt
            });
            await _dbContext.SaveChangesAsync();

            return Results.Ok(new AuthResponse
                (user.Id, user.Email!, roles, accessToken, accessExpiresAt, refreshToken, refreshExpireAt));
        }

        [HttpPost("refresh-token")]
        public async Task<IResult> Refresh(RefreshRequest request)
        {
            var existing = await _dbContext.RefreshTokens
                                    .FirstOrDefaultAsync(t => t.Token == request.RefreshToken);
            if (existing is null)
            {
                return Results.Unauthorized();
            }

            if (!existing.IsActive)
            {
                if (existing.Revoked is not null)
                {
                    await RevokeAllActiveTokensAsync(existing.UserId);
                }
                return Results.Unauthorized();
            }

            var user = await _userManager.FindByIdAsync(existing.UserId!);
            if (user is null)
            {
                return Results.Unauthorized();
            }

            var newRefreshToken = _tokenService.CreateRefreshToken();
            var refreshExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.Value.RefreshTokenDays);

            existing.Revoked = DateTime.UtcNow;
            existing.ReplacedByToken = newRefreshToken;

            _dbContext.RefreshTokens.Add(new RefreshToken
            {
                Token = newRefreshToken,
                UserId = user.Id,
                Created = DateTime.UtcNow,
                Expires = refreshExpiresAt
            });
            await _dbContext.SaveChangesAsync();

            var roles = await _userManager.GetRolesAsync(user);
            var (accessToken, accessExpiresAt) = _tokenService.CreateToken(user, roles);

            return Results.Ok(new AuthResponse(
                    user.Id, user.Email!, roles, accessToken, accessExpiresAt,
                    newRefreshToken, refreshExpiresAt));
        }

        private async Task RevokeAllActiveTokensAsync(string? userId)
        {
            var activeTokens = await _dbContext.RefreshTokens
                                               .Where(t => t.UserId == userId && t.Revoked == null)
                                               .ToListAsync();
            foreach (var token in activeTokens)
            {
                token.Revoked = DateTime.UtcNow;
            }
            await _dbContext.SaveChangesAsync();
        }

        [HttpPost("logout")]
        public async Task<IResult> Logout(RevokeRequest request)
        {
            var result = await RevokeAsync(request);
            await _signInManager.SignOutAsync();
            return result;
        }

        private async Task<IResult> RevokeAsync(RevokeRequest request)
        {
            var token = await _dbContext.RefreshTokens
                .FirstOrDefaultAsync(t => t.Token == request.RefreshToken);

            if (token is null || !token.IsActive)
            {
                return Results.NotFound("Token not found or already inactive.");
            }

            token.Revoked = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();

            return Results.Ok("Refresh token revoked.");
        }

        [HttpPost("register")]
        public async Task<IResult> Register(RegisterDto registerDto)
        {
            var user = new AppUser
            {
                FirstName = registerDto.FirstName,
                LastName = registerDto.Lastname,
                Email = registerDto.Email,
                UserName = registerDto.Email
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded) return Results.BadRequest($"Email '{registerDto.Email}' is already registered.");

            var roleresult = await _userManager.AddToRoleAsync(user, "Member");
            if (!roleresult.Succeeded) return Results.BadRequest($"Role does not exist.");

            return Results.Ok("User registerred successfully.");
        }
    }
}
