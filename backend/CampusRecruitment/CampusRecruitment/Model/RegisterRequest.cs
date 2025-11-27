namespace CampusRecruitment.Model
{
    public class RegisterRequest
    {
      

        public string Email { get; set; }

        public string FullName { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }   // "User" or "Admin" or "Employer"
    }
}
