using MakhzonAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient; 

namespace MakhzonAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private string connString = "Data Source=SQL5113.site4now.net;Initial Catalog=db_ac7979_makhzondb;User Id=db_ac7979_makhzondb_admin;Password=Bahi.2026;Encrypt=True;TrustServerCertificate=True;";

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                using (var conn = new SqlConnection(connString))
                {
                    await conn.OpenAsync();
                    // البحث عن المستخدم بالإيميل والباسورد
                    string sql = "SELECT Id, Name, Email, Role FROM Users WHERE (Email = @identity OR Name = @identity) AND Password = @pass";
                    var cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@identity", request.Email);
                    cmd.Parameters.AddWithValue("@pass", request.Password);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            // إذا وجدنا المستخدم، نرجع بياناته (بدون الباسورد للأمان)
                            var user = new User
                            {
                                Id = (int)reader["Id"],
                                Name = reader["Name"].ToString(),
                                Email = reader["Email"].ToString(),
                                Role = reader["Role"].ToString()
                            };
                            return Ok(user);
                        }
                    }
                }
                return Unauthorized(new { message = "Email ou Mot de passe incorrect" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // كلاس صغير لاستقبال بيانات الدخول
        public class LoginRequest
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = new List<User>();
            using (var conn = new SqlConnection(connString))
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand("SELECT Id, Name, Email, Role FROM Users", conn);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        users.Add(new User
                        {
                            Id = (int)reader["Id"],
                            Name = reader["Name"].ToString(),
                            Email = reader["Email"].ToString(),
                            Role = reader["Role"].ToString()
                        });
                    }
                }
            }
            return Ok(users);
        }

        [HttpPost]
        public async Task<IActionResult> AddUser(User user)
        {
            using (var conn = new SqlConnection(connString))
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand("INSERT INTO Users (Name, Email, Password, Role) VALUES (@name, @email, @pass, @role)", conn);
                cmd.Parameters.AddWithValue("@name", user.Name);
                cmd.Parameters.AddWithValue("@email", user.Email);
                cmd.Parameters.AddWithValue("@pass", user.Password); // في الإنتاج يفضل تشفيرها
                cmd.Parameters.AddWithValue("@role", user.Role);
                await cmd.ExecuteNonQueryAsync();
            }
            return Ok(new { message = "User added" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            using (var conn = new SqlConnection(connString))
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand("DELETE FROM Users WHERE Id = @id", conn);
                cmd.Parameters.AddWithValue("@id", id);
                await cmd.ExecuteNonQueryAsync();
            }
            return Ok(new { message = "User deleted" });
        }

        
    }
}
