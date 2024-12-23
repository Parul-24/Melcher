using Login.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Login.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {

        private readonly IConfiguration _configuration;
        public LoginController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel login)
        {
            var username = _configuration["JwtSettings:Username"];
            var password = _configuration["JwtSettings:Password"];

            if (login.Username == username && login.Password == password)
            {
                var token = GenerateJwtToken(login.Username);
                return Ok(new
                {
                    success = true,
                    message = "Login successful!",
                    token
                });
            }
            else
            {
                return Unauthorized(new
                {
                    success = false,
                    message = "Invalid username or password."
                });
            }
        }

        private string GenerateJwtToken(string username)
        {
            var sessionId = Guid.NewGuid().ToString();

            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("SessionID", sessionId) 
            };

            var sKey = _configuration["JwtSettings:SecretKey"];
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(sKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "Melcher Assignment",
                audience: "API link",
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

}

}
