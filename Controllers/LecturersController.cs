using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FYP_Link.Data;
using FYP_Link.Models;
using FYP_Link.Services;

namespace FYP_Link.Controllers
{
    [Authorize(Roles = "Admin,Committee,Student")]
    [ApiController]
    [Route("api/[controller]")]
    public class LecturersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMailService _mailService;

        public class CreateLecturerDto
        {
            public string Name { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string StaffId { get; set; } = string.Empty;
            public string Department { get; set; } = string.Empty;
            public string CurrentPosition { get; set; } = string.Empty;
        }

        public class LecturerResponseDto
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public string StaffId { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string Department { get; set; } = string.Empty;
            public string CurrentPosition { get; set; } = string.Empty;
            public string? Domain { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }

        public LecturersController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IMailService mailService)
        {
            _context = context;
            _userManager = userManager;
            _mailService = mailService;
        }

        // GET: api/lecturers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LecturerResponseDto>>> GetLecturers()
        {
            var lecturers = await _context.Lecturers
                .Include(l => l.ApplicationUser)
                .OrderBy(l => l.Name)
                .Select(l => new LecturerResponseDto
                {
                    Id = l.Id,
                    Name = l.Name,
                    StaffId = l.StaffId,
                    Email = l.ApplicationUser.Email!,
                    Department = l.Department,
                    CurrentPosition = l.CurrentPosition,
                    Domain = l.Domain,
                    CreatedAt = l.CreatedAt,
                    UpdatedAt = l.UpdatedAt
                })
                .ToListAsync();

            return lecturers;
        }

        // GET: api/lecturers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LecturerResponseDto>> GetLecturer(int id)
        {
            var lecturer = await _context.Lecturers
                .Include(l => l.ApplicationUser)
                .Where(l => l.Id == id)
                .Select(l => new LecturerResponseDto
                {
                    Id = l.Id,
                    Name = l.Name,
                    StaffId = l.StaffId,
                    Email = l.ApplicationUser.Email!,
                    Department = l.Department,
                    CurrentPosition = l.CurrentPosition,
                    Domain = l.Domain,
                    CreatedAt = l.CreatedAt,
                    UpdatedAt = l.UpdatedAt
                })
                .FirstOrDefaultAsync();

            if (lecturer == null)
            {
                return NotFound(new { Message = "Lecturer not found" });
            }

            return lecturer;
        }

        // POST: api/lecturers
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<object>> CreateLecturer(CreateLecturerDto dto)
        {
            // Check if email is already in use
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
            {
                return BadRequest(new { Message = "Email is already in use" });
            }

            // Check if StaffId is already in use
            var existingStaffId = await _context.Lecturers.AnyAsync(l => l.StaffId == dto.StaffId);
            if (existingStaffId)
            {
                return BadRequest(new { Message = "Staff ID is already in use" });
            }

            // Create the user account with default password
            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                EmailConfirmed = true // Auto-confirm for simplicity
            };

            var result = await _userManager.CreateAsync(user, "Password123!");
            if (!result.Succeeded)
            {
                return BadRequest(new { Message = "Failed to create user account", Errors = result.Errors });
            }

            // Assign the Supervisor role
            var roleResult = await _userManager.AddToRoleAsync(user, "Supervisor");
            if (!roleResult.Succeeded)
            {
                // If role assignment fails, delete the user and return error
                await _userManager.DeleteAsync(user);
                return BadRequest(new { Message = "Failed to assign supervisor role" });
            }

            // Create the lecturer profile with capitalized name
            var lecturer = new Lecturer
            {
                Name = dto.Name.ToUpper(),
                StaffId = dto.StaffId,
                Department = dto.Department,
                CurrentPosition = dto.CurrentPosition,
                ApplicationUserId = user.Id,
                CreatedAt = DateTime.UtcNow
            };

            _context.Lecturers.Add(lecturer);
            await _context.SaveChangesAsync();

            // Send welcome email
            try
            {
                await _mailService.SendWelcomeEmailAsync(dto.Email, dto.Name);
            }
            catch (Exception ex)
            {
                // Log the error but don't fail the request
                Console.WriteLine($"Failed to send welcome email: {ex.Message}");
            }

            return Ok(new
            {
                Message = "Lecturer account created successfully. A welcome email has been sent.",
                Lecturer = new LecturerResponseDto
                {
                    Id = lecturer.Id,
                    Name = lecturer.Name,
                    StaffId = lecturer.StaffId,
                    Email = user.Email!,
                    Department = lecturer.Department,
                    CurrentPosition = lecturer.CurrentPosition,
                    CreatedAt = lecturer.CreatedAt,
                    UpdatedAt = lecturer.UpdatedAt
                }
            });
        }

        // PUT: api/lecturers/5
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLecturer(int id, CreateLecturerDto dto)
        {
            var lecturer = await _context.Lecturers
                .Include(l => l.ApplicationUser)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (lecturer == null)
            {
                return NotFound(new { Message = "Lecturer not found" });
            }

            // Check if the new email is already in use by another user
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null && existingUser.Id != lecturer.ApplicationUserId)
            {
                return BadRequest(new { Message = "Email is already in use" });
            }

            // Check if the new StaffId is already in use by another lecturer
            var existingStaffId = await _context.Lecturers
                .AnyAsync(l => l.StaffId == dto.StaffId && l.Id != id);
            if (existingStaffId)
            {
                return BadRequest(new { Message = "Staff ID is already in use" });
            }

            // Update lecturer profile
            lecturer.Name = dto.Name.ToUpper();
            lecturer.StaffId = dto.StaffId;
            lecturer.Department = dto.Department;
            lecturer.CurrentPosition = dto.CurrentPosition;
            lecturer.UpdatedAt = DateTime.UtcNow;

            // Update user email if it changed
            if (lecturer.ApplicationUser.Email != dto.Email)
            {
                lecturer.ApplicationUser.Email = dto.Email;
                lecturer.ApplicationUser.UserName = dto.Email;
                lecturer.ApplicationUser.NormalizedEmail = dto.Email.ToUpper();
                lecturer.ApplicationUser.NormalizedUserName = dto.Email.ToUpper();
            }

            await _context.SaveChangesAsync();

            return Ok(new LecturerResponseDto
            {
                Id = lecturer.Id,
                Name = lecturer.Name,
                StaffId = lecturer.StaffId,
                Email = lecturer.ApplicationUser.Email!,
                Department = lecturer.Department,
                CurrentPosition = lecturer.CurrentPosition,
                CreatedAt = lecturer.CreatedAt,
                UpdatedAt = lecturer.UpdatedAt
            });
        }

        // DELETE: api/lecturers/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLecturer(int id)
        {
            var lecturer = await _context.Lecturers
                .Include(l => l.ApplicationUser)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (lecturer == null)
            {
                return NotFound(new { Message = "Lecturer not found" });
            }

            // Delete the user account (this will cascade delete the lecturer profile)
            var user = await _userManager.FindByIdAsync(lecturer.ApplicationUserId);
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }

            return NoContent();
        }
        public class UpdateDomainDto
        {
            public string Domain { get; set; } = string.Empty;
        }

        // PUT: api/lecturers/{id}/domain
        [Authorize(Roles = "Committee")]
        [HttpPut("{id}/domain")]
        public async Task<IActionResult> UpdateLecturerDomain(int id, [FromBody] UpdateDomainDto dto)
        {
            var lecturer = await _context.Lecturers.FindAsync(id);

            if (lecturer == null)
            {
                return NotFound(new { Message = "Lecturer not found" });
            }

            lecturer.Domain = dto.Domain;
            lecturer.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}