namespace CampusRecruitmentBackend.Models
{
    public class StudentEducation
    {
        public int EducationId { get; set; }
        public int StudentId { get; set; }

        public string UniversityName { get; set; } = string.Empty;
        public string Degree { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;

        public int StartYear { get; set; }
        public int EndYear { get; set; }

        public string CurrentSemester { get; set; } = string.Empty;

        public string Skills { get; set; } = string.Empty;
        public decimal CGPA { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsActive { get; set; }
    }
}
