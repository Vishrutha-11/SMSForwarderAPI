using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using PaymentDetails.Models;
using System.Data;

namespace PaymentDetails.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentDeatilsController : Controller

    {
        private readonly IConfiguration _configuration;

        public PaymentDeatilsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // ================= GET API =================

        [HttpGet]
        public IActionResult Get(
            [FromQuery] int submitDate,
            [FromQuery] int submitMonth,
            [FromQuery] int submitYear,
            [FromQuery] DateTime? monthYear)
        {
            var list = new List<RakePaymentDetail>();

            using (SqlConnection con = new SqlConnection(
                _configuration.GetConnectionString("SQLConnection")))
            using (SqlCommand cmd =
                new SqlCommand("GetRakePaymentDetails", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@SubmitDate", SqlDbType.Int)
                    .Value = submitDate;

                cmd.Parameters.Add("@SubmitMonth", SqlDbType.Int)
                    .Value = submitMonth;

                cmd.Parameters.Add("@SubmitYear", SqlDbType.Int)
                    .Value = submitYear;

                cmd.Parameters.Add("@MonthYear", SqlDbType.DateTime)
                    .Value = (object?)monthYear ?? DBNull.Value;

                con.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new RakePaymentDetail
                        {
                            SlNo = reader["SlNo"] == DBNull.Value ? 0 : Convert.ToInt32(reader["SlNo"]),

                            BillNo = reader["BillNo"] == DBNull.Value ? 0 : Convert.ToInt32(reader["BillNo"]),

                            Place = reader["Place"]?.ToString() ?? "",

                            PassDate = reader["PassDate"] == DBNull.Value
        ? null
        : Convert.ToDateTime(reader["PassDate"]),

                            Barcode = reader["Barcode"]?.ToString() ?? "",

                            MonthYear = reader["MonthYear"] == DBNull.Value
        ? null
        : Convert.ToDateTime(reader["MonthYear"]),

                            ActualSubmitDate = reader["ActualSubmitDate"] == DBNull.Value
        ? null
        : Convert.ToDateTime(reader["ActualSubmitDate"]),

                            RakeNo = reader["RakeNo"]?.ToString() ?? "",

                            RakeTypeName = reader["RakeTypeName"]?.ToString() ?? "",

                           

                            Tonns = reader["Tonns"] == DBNull.Value
        ? 0
        : Convert.ToDecimal(reader["Tonns"]),

                            Amount = reader["Amount"] == DBNull.Value
        ? 0
        : Convert.ToDecimal(reader["Amount"]),

                            Payment = reader["Payment"] == DBNull.Value
        ? 0
        : Convert.ToDecimal(reader["Payment"]),

                            IsTDS = reader["IsTDS"] == DBNull.Value
        ? 0
        : Convert.ToInt32(reader["IsTDS"]),

                            TDS = reader["TDS"] == DBNull.Value
        ? 0
        : Convert.ToInt32(reader["TDS"]),

                            GST = reader["GST"] == DBNull.Value
        ? 0
        : Convert.ToInt32(reader["GST"]),

                            ExtShortage = reader["ExtShortage"] == DBNull.Value
        ? 0
        : Convert.ToDecimal(reader["ExtShortage"]),

                            Shortage = reader["Shortage"] == DBNull.Value
        ? 0
        : Convert.ToDecimal(reader["Shortage"]),

                            Remarks = reader["Remarks"]?.ToString() ?? "",

                            CustomerId = reader["CustomerId"] == DBNull.Value
        ? 0
        : Convert.ToInt32(reader["CustomerId"]),

                            UB = reader["UB"] == DBNull.Value
        ? 0
        : Convert.ToInt32(reader["UB"])
                        });
                    }
                }
            }

            return Ok(list);
        }


            [HttpPost("Update")]
        public IActionResult Update(
    [FromBody] List<RakePaymentDetail> details)
        {
                if (details == null || details.Count == 0)
                    return BadRequest("No data received");

                try
                {
                    using SqlConnection con =
                        new SqlConnection(_configuration.GetConnectionString("SQLConnection"));

                    con.Open();

                    foreach (var item in details)
                    {
                        using SqlCommand cmd =
                            new SqlCommand("UpdateRakePaymentDetails", con);

                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@BillNo", item.BillNo);
                        cmd.Parameters.AddWithValue("@Payment", item.Payment);
                        cmd.Parameters.AddWithValue("@TDS", item.TDS);
                        cmd.Parameters.AddWithValue("@IsTDS", item.IsTDS);
                        cmd.Parameters.AddWithValue("@Shortage", item.Shortage);

                        cmd.Parameters.AddWithValue("@Remarks",
                            (object?)item.Remarks ?? DBNull.Value);

                        cmd.Parameters.AddWithValue("@PassDate",
                            (object?)item.PassDate ?? DBNull.Value);

                        cmd.Parameters.AddWithValue("@SubmitDate",
                            (object?)item.SubmitDate ?? DBNull.Value);

                        cmd.Parameters.AddWithValue("@SubmitMonth",
                            (object?)item.SubmitMonth ?? DBNull.Value);

                        cmd.Parameters.AddWithValue("@SubmitYear",
                            (object?)item.SubmitYear ?? DBNull.Value);

                        cmd.Parameters.AddWithValue("@Barcode",
                            (object?)item.Barcode ?? DBNull.Value);

                    cmd.Parameters.AddWithValue("@ActualSubmitDate",
                        (object?)item.ActualSubmitDate ?? DBNull.Value);


                    cmd.Parameters.AddWithValue("@CustomerId", item.CustomerId);
                    cmd.Parameters.AddWithValue("@UB", item.UB);


                    cmd.ExecuteNonQuery();
                    }

                    return Ok(new
                    {
                        Message = "Updated Successfully"
                    });
                }
                catch (Exception ex)
                {
                    return BadRequest(new
                    {
                        Error = ex.Message
                    });
                }
            }
        }
    }
   