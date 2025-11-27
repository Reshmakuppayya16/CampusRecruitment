using CampusRecruitment.Data;
using CampusRecruitment.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

namespace CampusRecruitment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly DatabaseHelper _db;

        public AuthController(DatabaseHelper db)
        {
            _db = db;
        }

        // =========================== REGISTER API ===========================
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequest request)
        {
            if (request == null) return BadRequest("Invalid registration data");

            string passwordHash = HashPassword(request.PasswordHash);

            try
            {
                using SqlConnection conn = _db.GetConnection();
                conn.Open();

                // Check duplicate email
                using SqlCommand checkCmd = new SqlCommand(
                    "SELECT COUNT(*) FROM Users WHERE Email=@Email", conn);
                checkCmd.Parameters.AddWithValue("@Email", request.Email);

                if ((int)checkCmd.ExecuteScalar() > 0)
                    return Conflict(new { message = "Email already registered" });

                // ================= Insert into Users Table =================
                string insertUser = @"
                    DECLARE @currentDate DATETIME = GETDATE();

                    INSERT INTO Users (FullName, Email, PasswordHash, Role, CreatedAt, UpdatedAt, IsActive)
                    VALUES (@FullName, @Email, @PasswordHash, @Role, @currentDate, @currentDate, 1);

                    SELECT SCOPE_IDENTITY();
                ";

                using SqlCommand userCmd = new SqlCommand(insertUser, conn);
                userCmd.Parameters.AddWithValue("@FullName", request.FullName);
                userCmd.Parameters.AddWithValue("@Email", request.Email);
                userCmd.Parameters.AddWithValue("@PasswordHash", passwordHash);
                userCmd.Parameters.AddWithValue("@Role", request.Role);

                int userId = Convert.ToInt32(userCmd.ExecuteScalar());

                // ================= Insert into respective Role table =================
                string tableInsertQuery = request.Role switch
                {
                    "Student" => @"
                        DECLARE @currentDate DATETIME = GETDATE();

                        INSERT INTO Student (UserId, Role, CreatedAt, UpdatedDate, IsActive)
                        VALUES (@UserId, @Role, @currentDate, @currentDate, 1)
                    ",
                    "Employer" => @"
                        DECLARE @currentDate DATETIME = GETDATE();

                        INSERT INTO Employer (UserId, Role, CreatedAt, UpdatedDate, IsActive)
                        VALUES (@UserId, @Role, @currentDate, @currentDate, 1)
                    ",
                    "Admin" => @"
                        DECLARE @currentDate DATETIME = GETDATE();

                        INSERT INTO Admin (UserId, Role, CreatedDate, UpdatedDate, IsActive)
                        VALUES (@UserId, @Role, @currentDate, @currentDate, 1)
                    ",
                    _ => null
                };

                if (!string.IsNullOrEmpty(tableInsertQuery))
                {
                    using SqlCommand roleCmd = new SqlCommand(tableInsertQuery, conn);
                    roleCmd.Parameters.AddWithValue("@UserId", userId);
                    roleCmd.Parameters.AddWithValue("@Role", request.Role);
                    roleCmd.ExecuteNonQuery();
                }

                return Ok(new { message = "Registration successful!", userId, request.Role });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Server error", error = ex.Message });
            }
        }

        // =========================== LOGIN API ===========================
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            if (request == null) return BadRequest("Invalid login data");

            try
            {
                using SqlConnection conn = _db.GetConnection();
                conn.Open();

                string query = @"
                    SELECT UserId, FullName, Email, PasswordHash, Role, IsActive 
                    FROM Users WHERE Email=@Email
                ";

                using SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Email", request.Email);

                using SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    if (!(bool)reader["IsActive"])
                        return BadRequest(new { message = "Account is deactivated." });

                    string enteredHash = HashPassword(request.PasswordHash);
                    string storedHash = reader["PasswordHash"].ToString();

                    if (enteredHash != storedHash)
                        return BadRequest(new { message = "Invalid password." });

                    return Ok(new
                    {
                        message = "Login successful",
                        user = new
                        {
                            UserId = reader["UserId"],
                            FullName = reader["FullName"],
                            Email = reader["Email"],
                            Role = reader["Role"]
                        }
                    });
                }

                return BadRequest(new { message = "Email not found." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Server error", error = ex.Message });
            }
        }

        // ======================= PASSWORD HASH HELPER =======================
        private string HashPassword(string password)
        {
            using SHA256 sha = SHA256.Create();
            return BitConverter.ToString(
                sha.ComputeHash(Encoding.UTF8.GetBytes(password))
            ).Replace("-", "").ToLower();
        }
    }
}
