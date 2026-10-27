using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;

namespace NewAPI.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class SMSForwardingController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly string GPSTrackConnectionconnection;
        public SMSForwardingController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
          //  GPSTrackConnectionconnection = configuration.GetConnectionString("GPSTrackConnection");
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                List<SMSForwardingConfigModel> list = new List<SMSForwardingConfigModel>();

                using (SqlConnection con = new SqlConnection((_connectionString)))
                {
                    using (SqlCommand cmd = new SqlCommand("USP_GetAllSMSForwardingConfig", con))
                    {
                        await con.OpenAsync();

                        SqlDataReader reader = await cmd.ExecuteReaderAsync();

                        while (await reader.ReadAsync())
                        {
                            list.Add(new SMSForwardingConfigModel
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                FilterName = reader["FilterName"].ToString(),
                                FilterType = reader["FilterType"].ToString(),
                                ContactNumbers = reader["ContactNumbers"].ToString(),
                                ForwardingMethod = reader["ForwardingMethod"].ToString(),
                                ForwardingDuration = reader["ForwardingDuration"].ToString(),
                                IsEnabled = Convert.ToBoolean(reader["IsEnabled"])
                            });
                        }
                    }
                }
                return Ok(list);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Status = 500,
                    Success = false,
                    Message = ex.Message
                });
            }
        }
        [HttpPost("Insert")]
        public async Task<IActionResult> Save([FromBody] SMSForwardingConfigModel model)
        {
            try
            {
                using (SqlConnection con = new SqlConnection((_connectionString)))
                {
                    using (SqlCommand cmd = new SqlCommand("USP_SaveSMSForwardingConfig", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@FilterName", model.FilterName);
                        cmd.Parameters.AddWithValue("@FilterType", model.FilterType);
                        cmd.Parameters.AddWithValue("@ContactNumbers", model.ContactNumbers);
                        cmd.Parameters.AddWithValue("@ForwardingMethod", model.ForwardingMethod);
                        cmd.Parameters.AddWithValue("@ForwardingDuration", model.ForwardingDuration);
                        cmd.Parameters.AddWithValue("@IsEnabled", model.IsEnabled);

                        await con.OpenAsync();

                        model.Id = Convert.ToInt32(await cmd.ExecuteScalarAsync());

                        return Ok(model);
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Status = 500,
                    Success = false,
                    Message = ex.Message
                });
            }
        }



        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromBody] SMSForwardingConfigModel model)
        {
            Console.WriteLine("API ID : " + model.Id);
            Console.WriteLine("API Duration : " + model.ForwardingDuration);
            Console.WriteLine("API Enabled : " + model.IsEnabled);
            try
            {
                using (SqlConnection con = new SqlConnection((_connectionString)))
                {
                    using (SqlCommand cmd = new SqlCommand("USP_UpdateSMSForwardingConfig", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@Id", model.Id);
                        cmd.Parameters.AddWithValue("@FilterName", model.FilterName);
                        cmd.Parameters.AddWithValue("@FilterType", model.FilterType);
                        cmd.Parameters.AddWithValue("@ContactNumbers", model.ContactNumbers);
                        cmd.Parameters.AddWithValue("@ForwardingMethod", model.ForwardingMethod);
                        cmd.Parameters.AddWithValue("@ForwardingDuration", model.ForwardingDuration ?? "Never");
                        cmd.Parameters.AddWithValue("@IsEnabled", model.IsEnabled);

                        await con.OpenAsync();


                        int rows = Convert.ToInt32(
                            await cmd.ExecuteScalarAsync()
                        );


                        return Ok(new
                        {
                            Success = true,
                            RowsAffected = rows
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Status = 500,
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                using SqlConnection con =
                    new SqlConnection((_connectionString));

                using SqlCommand cmd =
                    new SqlCommand("USP_DeleteSMSForwardingConfig", con);

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);

                await con.OpenAsync();

                int rows = Convert.ToInt32(await cmd.ExecuteScalarAsync());

                return Ok(new
                {
                    Success = true,
                    RowsAffected = rows
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }
    }



    public class SMSForwardingConfigModel
    {
        public int Id { get; set; }
        public string FilterName { get; set; }
        public string FilterType { get; set; }
        public string ContactNumbers { get; set; }
        public string ForwardingMethod { get; set; }
        public string? ForwardingDuration { get; set; }
        public DateTime? ExpireAt { get; set; }
        public bool IsEnabled { get; set; }
    }


}

