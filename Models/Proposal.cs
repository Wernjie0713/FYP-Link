using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FYP_Link.Models
{
    public class Proposal
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Abstract { get; set; } = string.Empty;

        [Required]
        public string ProjectType { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string AcademicSession { get; set; } = string.Empty; // e.g. 2024/2025

        [Required]
        public int Semester { get; set; } // 1 or 2
        
        // Path to uploaded PDF file (if any)
        public string? PdfFilePath { get; set; }

        // Foreign keys
        [Required]
        public int StudentId { get; set; }
        [ForeignKey("StudentId")]
        public Student Student { get; set; } = null!;

        [Required]
        public int SupervisorId { get; set; }
        [ForeignKey("SupervisorId")]
        public Lecturer Supervisor { get; set; } = null!;

        public ICollection<Evaluation> Evaluations { get; set; } = new List<Evaluation>();
    }
}