using System;

namespace FYP_Link.Models
{
    public class CommitteeMembership
    {
        public int LecturerId { get; set; }
        public int AcademicProgramId { get; set; }

        // Navigation properties
        public Lecturer? Lecturer { get; set; }
        public AcademicProgram? AcademicProgram { get; set; }
    }
}