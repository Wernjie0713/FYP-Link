using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FYP_Link.Data;
using FYP_Link.Models;
using FYP_Link.Services;

namespace FYP_Link.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IMailService _mailService;
        private readonly ApplicationDbContext _context;

        public class RegisterDto
        {
            public string Email { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
            public string Name { get; set; } = string.Empty;
            public string MatricNumber { get; set; } = string.Empty;
        }

        public class LoginDto
        {
            public string Email { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }

        public AuthController(
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration,
            IMailService mailService,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _configuration = configuration;
            _mailService = mailService;
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if matriculation number is already in use
            var existingStudent = await _context.Students
                .FirstOrDefaultAsync(s => s.MatricNumber == registerDto.MatricNumber);
            if (existingStudent != null)
            {
                return BadRequest(new { Message = "Matriculation Number is already in use" });
            }

            var userExists = await _userManager.FindByEmailAsync(registerDto.Email);
            if (userExists != null)
            {
                return BadRequest(new { Message = "User with this email already exists" });
            }

            var user = new ApplicationUser
            {
                UserName = registerDto.Email,
                Email = registerDto.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                EmailConfirmed = false // Set to false initially
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                return BadRequest(new { Message = "User creation failed", Errors = result.Errors });
            }

            // Assign the Student role
            var roleResult = await _userManager.AddToRoleAsync(user, "Student");
            if (!roleResult.Succeeded)
            {
                // If role assignment fails, delete the user and return error
                await _userManager.DeleteAsync(user);
                return BadRequest(new { Message = "Failed to assign student role", Errors = roleResult.Errors });
            }

            // Create and save the student record with capitalized name
            var student = new Student
            {
                Name = registerDto.Name.ToUpper(),
                MatricNumber = registerDto.MatricNumber,
                ApplicationUserId = user.Id
            };

            try
            {
                _context.Students.Add(student);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // If student record creation fails, delete the user and return error
                await _userManager.DeleteAsync(user);
                return BadRequest(new { Message = "Failed to create student record", Error = ex.Message });
            }

            // Generate email confirmation token
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var frontendBaseUrl = _configuration["FrontendBaseUrl"];
            var confirmationLink = $"{frontendBaseUrl}/confirm-email?userId={user.Id}&token={System.Web.HttpUtility.UrlEncode(token)}";

            try
            {
                // Send verification email
                await _mailService.SendVerificationEmailAsync(user.Email!, registerDto.Name, confirmationLink);

                return Ok(new { Message = "Registration successful! Please check your email to verify your account." });
            }
            catch (Exception ex)
            {
                // Log the error but don't expose it to the user
                Console.WriteLine($"Failed to send verification email: {ex.Message}");
                return Ok(new { Message = "Registration successful! Please check your email to verify your account." });
            }
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                return BadRequest(new { Message = "Invalid email confirmation link" });
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound(new { Message = "User not found" });
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                return BadRequest(new { Message = "Failed to confirm email", Errors = result.Errors });
            }

            // Return a success message that the frontend can display
            return Ok(new { Message = "Email confirmed successfully! You can now log in." });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
            {
                return Unauthorized(new { Message = "Invalid email or password" });
            }

            // Check if email is confirmed for students
            if (!user.EmailConfirmed && await _userManager.IsInRoleAsync(user, "Student"))
            {
                return BadRequest(new { Message = "Please verify your email before logging in" });
            }

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!isPasswordValid)
            {
                return Unauthorized(new { Message = "Invalid email or password" });
            }

            // Get user roles
            var roles = await _userManager.GetRolesAsync(user);

            // Create claims for the token
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // Add role claims
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(1);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: credentials
            );

            return Ok(new { Token = new JwtSecurityTokenHandler().WriteToken(token) });
        }
    }
}