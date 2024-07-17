using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using EmployeeSalaryManagement.Models;
using EmployeeSalaryManagement.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;

namespace EmployeeSalaryManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly ApplicationDbContext _context;

        public AuthController(IConfiguration config, ApplicationDbContext context)
        {
            _config = config;
            _context = context;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await AuthenticateUserAsync(model.Username, model.Password);

            if (user != null)
            {
                var tokenString = GenerateJWTToken(user);
                Console.WriteLine($"Generated token for {model.Username}: {tokenString}");
                return Ok(new { Token = tokenString });
            }
            else
            {
                return Unauthorized(new { Error = "Invalid username or password." });
            }
        }



        [AllowAnonymous]
        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody] SignUpModel model)
        {
            // Validate input
            if (string.IsNullOrEmpty(model.Username))
            {
                ModelState.AddModelError("username", "The username field is required.");
            }
            if (string.IsNullOrEmpty(model.Password))
            {
                ModelState.AddModelError("password", "The password field is required.");
            }
            if (string.IsNullOrEmpty(model.Role))
            {
                ModelState.AddModelError("role", "The role field is required.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    status = 400,
                    errors = ModelState.ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToList()
                    ),
                    traceId = HttpContext?.TraceIdentifier ?? Activity.Current?.Id ?? Guid.NewGuid().ToString()

                });
            }

            // Check if username already exists
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == model.Username);
            if (existingUser != null)
            {
                return BadRequest("Username already exists.");
            }

            // Create new user
            var newUser = new User
            {
                Username = model.Username,
                Password = model.Password, 
                Role = model.Role
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return Ok("User created successfully.");
        }




        private async Task<User?> AuthenticateUserAsync(string username, string password)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == username && u.Password == password);
            return user;
        }


        private string GenerateJWTToken(User user)
        {
            var jwtKey = _config["Jwt:Key"];
            var jwtIssuer = _config["Jwt:Issuer"];
            var jwtAudience = _config["Jwt:Audience"];
            var jwtExpireMinutes = Convert.ToDouble(_config["Jwt:ExpireMinutes"]);

            if (string.IsNullOrEmpty(jwtKey) || string.IsNullOrEmpty(jwtIssuer) || string.IsNullOrEmpty(jwtAudience))
            {
                throw new ApplicationException("JWT configuration values are missing or empty.");
            }

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "User cannot be null.");
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Role, user.Role)
        };

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(jwtExpireMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
