namespace DatingApp.API.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;
    using System.Threading.Tasks;
    using AutoMapper;
    using DatingApp.API.Data;
    using DatingApp.API.Dtos;
    using DatingApp.API.Models;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.IdentityModel.Tokens;

    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AuthenticationController(UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration config, IMapper mapper)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _mapper = mapper;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]UserRegisterDto userDto)
        {
            var userToCreate = _mapper.Map<User>(userDto);
            var result = await _userManager.CreateAsync(userToCreate, userDto.Password);

            var userToReturn = _mapper.Map<UserDetailsDto>(userToCreate);

            if (result.Succeeded)
            {
                return CreatedAtRoute("GetUser", new { Controller = "Users", id = userToCreate.Id }, userToReturn);
            }
            return BadRequest(result.Errors);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]UserLoginDto userDto)
        {
            var user = await _userManager.FindByNameAsync(userDto.Username);

            var result = await _signInManager.CheckPasswordSignInAsync(user, userDto.Password, false);

            if (result.Succeeded)
            {
                var appUser = await _userManager.Users.Include(p => p.Photos).FirstOrDefaultAsync(u => u.NormalizedUserName == userDto.Username.ToUpper());
                var userToReturn = _mapper.Map<UserListDto>(appUser);

                return Ok(new { token = GenerateJwtToken(appUser), user = userToReturn });
            }
            return Unauthorized();
        }

        private async Task<string> GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName)
            };

            var roles = await _userManager.GetRolesAsync(user);

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}