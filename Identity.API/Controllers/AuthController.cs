using AutoMapper;
using Identity.API.DTOs;
using Identity.API.Extensions;
using Identity.API.Interfaces;
using Identity.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Identity.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(UserManager<AppUser> userManager, 
                                SignInManager<AppUser> signInManager, 
                                ITokenService tokenService, 
                                IMapper mapper) : ControllerBase
    {
        private readonly SignInManager<AppUser> _signInManager = signInManager;
        private readonly ITokenService _tokenService = tokenService;
        private readonly IMapper _mapper = mapper;
        private readonly UserManager<AppUser> _userManager = userManager;

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
        [Authorize]
        [HttpGet("address")]
        public async Task<ActionResult<AddressDto>> GetUserAddress()
        {
            var user = await _userManager.FindByUserByClaimsPrincipleWithAddressAsync(HttpContext.User);

            return _mapper.Map<Address, AddressDto>(user!.Address);

        }
        [Authorize]
        [HttpPut("address")]
        public async Task<ActionResult<AddressDto>> UpdateUserAddress(AddressDto address)
        {
            var user = await _userManager.FindByUserByClaimsPrincipleWithAddressAsync(HttpContext.User);
            user!.Address = _mapper.Map<AddressDto, Address>(address);
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded) return Ok(_mapper.Map<Address, AddressDto>(user.Address));

            return BadRequest("Problem updating the user");
        }
        [HttpPost("login")]
        public async Task<IResult> Login(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null) return Results.Unauthorized();

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
            if (!result.Succeeded) return Results.Unauthorized();

            var roles = await _userManager.GetRolesAsync(user);
            var (Token, ExpiresAt) = _tokenService.CreateToken(user, roles);


            return Results.Ok(new AuthResponse(user.Id, user.Email!, roles, Token, ExpiresAt));
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
            if (!result.Succeeded) return Results.BadRequest($"Email '{registerDto.Email}' is already registered");

            var roleresult = await _userManager.AddToRoleAsync(user, "Member");
            if (!roleresult.Succeeded) return Results.BadRequest($"Role does not exist");

            return Results.Ok("User registerred successfully.");
        }
    }
}
