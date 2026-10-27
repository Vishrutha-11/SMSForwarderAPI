using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Payments.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminLoginController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AdminLoginController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("Login")]
        public IActionResult Login([FromBody] AdminLoginRequest request)
        {
            try
            {
                string connectionString =
                    _configuration.GetConnectionString("SATConnection");

                using SqlConnection con =
                    new SqlConnection(connectionString);

                using SqlCommand cmd =
                    new SqlCommand("AdminLogin", con);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue(
                    "@Username",
                    request.Username
                );

                cmd.Parameters.AddWithValue(
                    "@Password",
                    request.Password
                );

                con.Open();

                using SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    return Ok(new
                    {
                        success = true,
                        message = "Login successful",
                        adminId = reader["AdminId"],
                        username = reader["Username"]
                    });
                }

                return Unauthorized(new
                {
                    success = false,
                    message = "Invalid username or password"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }
        [HttpPost("ChangePassword")]
        public IActionResult ChangePassword(
    [FromBody] ChangePasswordRequest request)
        {
            try
            {
                string connectionString =
                    _configuration.GetConnectionString("SATConnection");

                using SqlConnection con =
                    new SqlConnection(connectionString);

                using SqlCommand cmd =
                    new SqlCommand("ChangeAdminPassword", con);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue(
                    "@Username",
                    request.Username
                );

                cmd.Parameters.AddWithValue(
                    "@OldPassword",
                    request.OldPassword
                );

                cmd.Parameters.AddWithValue(
                    "@NewPassword",
                    request.NewPassword
                );

                con.Open();

                using SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    bool success =
                        Convert.ToInt32(reader["Success"]) == 1;

                    if (success)
                    {
                        return Ok(new
                        {
                            success = true,
                            message = reader["Message"].ToString()
                        });
                    }

                    return BadRequest(new
                    {
                        success = false,
                        message = reader["Message"].ToString()
                    });
                }

                return BadRequest(new
                {
                    success = false,
                    message = "Unable to change password"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }
    }

    public class AdminLoginRequest
    {
        public string Username { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
    }
    public class ChangePasswordRequest
    {
        public string Username { get; set; } = string.Empty;

        public string OldPassword { get; set; } = string.Empty;

        public string NewPassword { get; set; } = string.Empty;
    }
}