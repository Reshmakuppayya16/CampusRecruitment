namespace CampusRecruitmentBackend.Models
{
    public class StudentPersonalInfo
    {
        public int PersonalId { get; set; }
        public int StudentId { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public DateTime CreateAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsActive { get; set; }

    }
}
