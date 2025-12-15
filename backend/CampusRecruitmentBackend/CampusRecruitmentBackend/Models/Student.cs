namespace CampusRecruitmentBackend.Models
{
    public class Student
    {
        public int StudentId { get; set; }
        public int UserId { get; set; }

        public string Role { get; set; } = "Student";

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsActive { get; set; }

        //From user table
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
