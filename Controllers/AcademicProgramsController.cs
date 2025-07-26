using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FYP_Link.Data;
using FYP_Link.Models;

namespace FYP_Link.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class AcademicProgramsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AcademicProgramsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/academicprograms
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AcademicProgram>>> GetPrograms()
        {
            return await _context.AcademicPrograms
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        // GET: api/academicprograms/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AcademicProgram>> GetProgram(int id)
        {
            var program = await _context.AcademicPrograms.FindAsync(id);

            if (program == null)
            {
                return NotFound();
            }

            return program;
        }

        // POST: api/academicprograms
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<AcademicProgram>> CreateProgram([FromBody] AcademicProgram program)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            program.CreatedAt = DateTime.UtcNow;
            _context.AcademicPrograms.Add(program);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetProgram),
                new { id = program.Id },
                program
            );
        }

        // PUT: api/academicprograms/5
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProgram(int id, [FromBody] AcademicProgram program)
        {
            if (id != program.Id)
            {
                return BadRequest(new { Message = "ID mismatch" });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingProgram = await _context.AcademicPrograms.FindAsync(id);
            if (existingProgram == null)
            {
                return NotFound(new { Message = "Program not found" });
            }

            // Update only allowed fields
            existingProgram.Name = program.Name;
            existingProgram.Description = program.Description;
            existingProgram.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProgramExists(id))
                {
                    return NotFound(new { Message = "Program not found" });
                }
                throw;
            }

            return Ok(existingProgram);
        }

        // DELETE: api/academicprograms/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProgram(int id)
        {
            var program = await _context.AcademicPrograms.FindAsync(id);
            if (program == null)
            {
                return NotFound(new { Message = "Program not found" });
            }

            _context.AcademicPrograms.Remove(program);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProgramExists(int id)
        {
            return _context.AcademicPrograms.Any(e => e.Id == id);
        }
    }
}