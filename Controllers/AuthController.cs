using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using StudentEnrollment.Api.Models;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace StudentEnrollment.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _config;

        public AuthController(UserManager<IdentityUser> userManager, IConfiguration config)
        {
            _userManager = userManager;
            _config = config;
        }

        //Register 
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var user = new IdentityUser
                {
                    UserName = model.Username,
                    Email = model.Username // using username as email for simplicity
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                    return Ok(new { message = "User registered successfully" });

                // send identity errors back to client
                return BadRequest(result.Errors.Select(e => e.Description));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        //login
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto model)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(model.Username);

                if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
                {
                    var token = GenerateJwtToken(user);
                    return Ok(new { token });
                }

                return Unauthorized(new { message = "Invalid login attempt" });
            }
            catch (Exception ex)
            {

                return Unauthorized(new { message = ex.Message });
            }

        }

        //token
        private string GenerateJwtToken(IdentityUser user)
        {

            var jwtKey = _config["Jwt:Key"] ?? "SiphosethuNogcazi";
            var jwtIssuer = _config["Jwt:Issuer"] ?? "StudentEnrollmentIssuer";

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email ?? ""),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName ?? "")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: null,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);


        }
    }
}
