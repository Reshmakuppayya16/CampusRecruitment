using Microsoft.AspNetCore.Mvc;
using CampusRecruitmentBackend.Models;
using CampusRecruitmentBackend.Data;
using Microsoft.Data.SqlClient;

namespace CampusRecruitmentBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly DatabaseHelper _db;

        public StudentController(DatabaseHelper db)
        {
            _db = db;
        }

    //////////////////////////////////////////////////////////////////////////////////

        //Get full profile by UserID
        [HttpGet("get-profile/{userId}")]
        public IActionResult GetProfile(int userId)
        {
            try
            {
                using (SqlConnection conn = _db.GetConnection())
                {
                    conn.Open();

                    string query = @"
                        SELECT 
                           s.StudentId, s.UserId, s.Role, 
                           pi.DateOfBirth, pi.Gender, pi.PhoneNumber,
                           edu.UniversityName, edu.Degree, edu.Department,
                           edu.StartYear, edu.EndYear, edu.CurrentSemester,
                           edu.Skills, edu.CGPA,
                           addr.Street, addr.City, addr.Pincode, addr.Country
                        FROM Student s
                        LEFT JOIN StudentPersonalInfo pi ON s.StudentId = pi.StudentId AND pi.IsActive = 1
                        LEFT JOIN StudentEducation edu ON s.StudentId = edu.StudentId AND edu.IsActive = 1
                        LEFT JOIN StudentAddress addr ON s.StudentId = addr.StudentId AND addr.IsActive = 1
                        WHERE s.UserId = @UserId AND s.IsActive = 1";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@UserId", userId);

                    SqlDataReader rd = cmd.ExecuteReader();
                    if (!rd.Read())
                        return NotFound("Student profile not found.");

                    var profile = new
                    {
                        StudentId = rd["StudentId"],
                        UserId = rd["UserId"],
                        DateOfBirth = rd["DateOfBirth"],
                        Gender = rd["Gender"],
                        PhoneNumber = rd["PhoneNumber"],
                        UniversityName = rd["UniversityName"],
                        Degree = rd["Degree"],
                        Department = rd["Department"],
                        StartYear = rd["StartYear"],
                        EndYear = rd["EndYear"],
                        CurrentSemester = rd["CurrentSemester"],
                        Skills = rd["Skills"],
                        CGPA = rd["CGPA"],
                        Street = rd["Street"],
                        City = rd["City"],
                        Pincode = rd["Pincode"],
                        Country = rd["Country"]
                    };

                    return Ok(profile);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Server error : {ex.Message}");
            }
        }

        //2. Insert personal information
        [HttpPost("save-personal/{userId}")]
        public IActionResult SavePersonalInfo(int userId, [FromBody] StudentPersonalInfo p)
        {
            try
            {
                using (SqlConnection conn = _db.GetConnection())
                {
                    conn.Open();

                    string findStudent = "SELECT StudentId FROM Student WHERE UserId = @UserId AND IsActive = 1";
                    SqlCommand findCmd = new SqlCommand(findStudent, conn);
                    findCmd.Parameters.AddWithValue("@UserId", userId);

                    object result = findCmd.ExecuteScalar();
                    if (result == null)
                        return BadRequest("Student account not found.");

                    int studentId = Convert.ToInt32(result);

                    //check if personal information exists
                    string check = "SELECT COUNT(*) FROM StudentPersonalInfo WHERE StudentId = @StudentId";
                    SqlCommand checkcmd = new SqlCommand(check, conn);
                    checkcmd.Parameters.AddWithValue("@StudentId", studentId);
                    int exists = (int)checkcmd.ExecuteScalar();

                    string query;

                    //update or insert
                    if (exists > 0)
                    {
                        query = @"UPDATE StudentPersonalInfo 
                                  SET DateOfBirth = @DateOfBirth, Gender = @Gender, PhoneNumber = @PhoneNumber,
                                      UpdatedAt = GETDATE()
                                  WHERE StudentId = @StudentId";
                    }
                    else
                    {
                        query = @"INSERT INTO StudentPersonalInfo
                                 (StudentId, DateOfBirth, Gender, PhoneNumber, CreatedAt, UpdatedAt, IsActive)
                                 VALUES (@StudentId, @DateOfBirth, @Gender, @PhoneNumber, GETDATE(), GETDATE(), 1)";
                    }

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    cmd.Parameters.AddWithValue("@DateOfBirth", (object?)p.DateOfBirth ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Gender", p.Gender ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@PhoneNumber", p.PhoneNumber ?? (object)DBNull.Value);

                    cmd.ExecuteNonQuery();
                    return Ok("Personal info saved successfully!");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        //3. Upsert education info
        [HttpPost("save-education/{userId}")]
        public IActionResult SaveEducation(int userId, [FromBody] StudentEducation e)
        {
            try
            {
                using (SqlConnection conn = _db.GetConnection())
                {
                    conn.Open();

                    string findStudent = "SELECT StudentId FROM Student WHERE UserId = @UserId";
                    SqlCommand findCmd = new SqlCommand(findStudent, conn);
                    findCmd.Parameters.AddWithValue("@UserId", userId);
                    object result = findCmd.ExecuteScalar();

                    if (result == null)
                        return BadRequest("Student account not found.");

                    int studentId = Convert.ToInt32(result);

                    string check = "SELECT COUNT(*) FROM StudentEducation WHERE StudentId = @StudentId";
                    SqlCommand checkCmd = new SqlCommand(check, conn);
                    checkCmd.Parameters.AddWithValue("@StudentId", studentId);
                    int exists = (int)checkCmd.ExecuteScalar();

                    string query;

                    if(exists > 0)
                    {
                        query = @"UPDATE StudentEducation SET
                                    UniversityName = @UniversityName,
                                    Degree=@Degree, Department=@Department,
                                    StartYear=@StartYear, EndYear=@EndYear,
                                    CurrentSemester=@CurrentSemester, Skills=@Skills,
                                    CGPA=@CGPA, UpdatedAt = GETDATE() WHERE 
                                    StudentId=@StudentId";
                    }
                    else
                    {
                        query = @"INSERT INTO StudentEducation
                                  (StudentId, UniversityName, Degree, Department, StartYear, EndYear,
                                   CurrentSemester, Skills, CGPA, CreatedAt, UpdatedAt, IsActive)
                                  VALUES (@StudentId, @UniversityName, @Degree, @Department,
                                          @StartYear, @EndYear, @CurrentSemester, @Skills, @CGPA, GETDATE(), GETDATE(), 1)";
                    }

                    SqlCommand cmd = new SqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    cmd.Parameters.AddWithValue("@UniversityName", e.UniversityName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Degree", e.Degree ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Department", e.Department ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@StartYear", (object?)e.StartYear ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@EndYear", (object?)e.EndYear ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@CurrentSemester", (object?)e.CurrentSemester ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Skills", e.Skills ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CGPA", (object?)e.CGPA ?? DBNull.Value);

                    cmd.ExecuteNonQuery();
                    return Ok("Education saved successfully!");
                }
            }
            catch(Exception ex)
            {

                return StatusCode(500, ex.Message);
            }
        }


        // ✅ 4. Upsert Address
        [HttpPost("save-address/{userId}")]
        public IActionResult SaveAddress(int userId, [FromBody] StudentAddress a)
        {
            try
            {
                using (SqlConnection conn = _db.GetConnection())
                {
                    conn.Open();

                    string findStudent = "SELECT StudentId FROM Student WHERE UserId = @UserId";
                    SqlCommand findCmd = new SqlCommand(findStudent, conn);
                    findCmd.Parameters.AddWithValue("@UserId", userId);
                    object result = findCmd.ExecuteScalar();

                    if (result == null)
                        return BadRequest("Student account not found.");

                    int studentId = Convert.ToInt32(result);

                    string check = "SELECT COUNT(*) FROM StudentAddress WHERE StudentId = @StudentId";
                    SqlCommand checkCmd = new SqlCommand(check, conn);
                    checkCmd.Parameters.AddWithValue("@StudentId", studentId);
                    int exists = (int)checkCmd.ExecuteScalar();

                    string query;

                    if (exists > 0)
                    {
                        query = @"UPDATE StudentAddress SET
                                    Street=@Street, City=@City, Pincode=@Pincode, Country=@Country,
                                    UpdatedAt = GETDATE()
                                  WHERE StudentId=@StudentId";
                    }
                    else
                    {
                        query = @"INSERT INTO StudentAddress
                                  (StudentId, Street, City, Pincode, Country, CreatedAt, UpdatedAt, IsActive)
                                  VALUES (@StudentId, @Street, @City, @Pincode, @Country, GETDATE(), GETDATE(), 1)";
                    }

                    SqlCommand cmd = new SqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    cmd.Parameters.AddWithValue("@Street", a.Street ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@City", a.City ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Pincode", a.Pincode ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Country", a.Country ?? (object)DBNull.Value);

                    cmd.ExecuteNonQuery();
                    return Ok("Address saved successfully!");
                }
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
