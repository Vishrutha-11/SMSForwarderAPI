using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System.Data;

namespace Payments.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InternalShiftingController : ControllerBase
    {
        private readonly string connectionString;

        public InternalShiftingController(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        [HttpPost("GetInternalShiftingBillID")]
        public IActionResult GetInternalShiftingBillID(
     [FromQuery] int submitDate,
     [FromQuery] int submitMonth,
     [FromQuery] int submitYear)
        {
            using SqlConnection con =
                new SqlConnection(connectionString);

            using SqlCommand cmd =
                new SqlCommand("GetInternalShiftingBillID", con);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@SubmitDate", submitDate);
            cmd.Parameters.AddWithValue("@SubmitMonth", submitMonth);
            cmd.Parameters.AddWithValue("@SubmitYear", submitYear);

            con.Open();

            SqlDataAdapter da =
                new SqlDataAdapter(cmd);

            DataSet ds =
                new DataSet();

            da.Fill(ds);

            var paymentDetails =
                ds.Tables[0].AsEnumerable()
                .Select(row => new
                {
                    SlNo = row["SlNo"] == DBNull.Value ? null : row["SlNo"],
                    BillNo = row["BillNo"] == DBNull.Value ? null : row["BillNo"],
                    Place = row["Place"] == DBNull.Value ? null : row["Place"],
                    Tonns = row["Tonns"] == DBNull.Value ? null : row["Tonns"],

                    Amount = row["Amount"] == DBNull.Value ? null : row["Amount"],

                    Payment = row["Payment"] == DBNull.Value
                        ? null
                        : row["Payment"],

                    isTDS = row["IsTDS"] == DBNull.Value ? null : row["isTDS"],

                    TDS = row["TDS"] == DBNull.Value
                        ? null
                        : row["TDS"],

                    GST = row["GST"] == DBNull.Value
                        ? null
                        : row["GST"],

                    Shortage = row["Shortage"] == DBNull.Value
                        ? null
                        : row["Shortage"],

                    Remarks = row["Remarks"] == DBNull.Value
                        ? null
                        : row["Remarks"],

                    Passdate = row["Passdate"] == DBNull.Value
                        ? null
                        : Convert.ToDateTime(row["Passdate"])
                            .ToString("yyyy-MM-dd"),

                    ExtShortage = row["ExtShortage"] == DBNull.Value
                        ? null
                        : row["ExtShortage"],

                    Barcode = row["Barcode"] == DBNull.Value
                        ? null
                        : row["Barcode"]
                });


            return Ok(new
            {
                PaymentDetails = paymentDetails,
                //ShortageDetails = shortageDetails
            });
        }




        [HttpPost("UpdateInternalShiftingBills")]
        public IActionResult UpdateInternalShiftingBills(
    [FromBody] InternalShiftingSaveRequest request)
        {
            
         
                using SqlConnection con =
                    new SqlConnection(connectionString);

                con.Open();

                foreach (var row in request.Rows)
                {

                    using SqlCommand cmd =
                        new SqlCommand("UpdateInternalshiftingBills", con);

                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@BillNo", row.BillNo);
                    cmd.Parameters.AddWithValue("@Payment", (object?)row.Payment ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsTDS", (object?)row.isTDS ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@TDS", (object?)row.TDS ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@GST", (object?)row.GST ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Shortage", (object?)row.Shortage ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Remarks",
                        string.IsNullOrWhiteSpace(row.Remarks)
                            ? DBNull.Value
                            : row.Remarks);

                    cmd.Parameters.AddWithValue("@PassDate",
                        (object?)row.PassDate ?? DBNull.Value);
                    cmd.Parameters.AddWithValue(
           "@Barcode",
           row.Barcode ?? "");

                    //cmd.Parameters.AddWithValue("@ExtShortage",row.ExtShortage);

                    cmd.Parameters.AddWithValue("@SubmitDate", request.SubmitDate.Day);
                    cmd.Parameters.AddWithValue("@SubmitMonth", request.SubmitDate.Month);
                    cmd.Parameters.AddWithValue("@SubmitYear", request.SubmitDate.Year);





                    cmd.ExecuteNonQuery();
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected == 0)
                    {
                        throw new Exception(
                            $"No row updated. BillNo={row.BillNo}");
                    }
                }

                using SqlCommand cmdReceived =
                    new SqlCommand(
                        "InsertInternalShiftingDetailsRecived",
                        con);

                cmdReceived.CommandType =
                    CommandType.StoredProcedure;

                cmdReceived.Parameters.AddWithValue(
                    "@BillDate",
                    request.SubmitDate.Date);

                cmdReceived.ExecuteNonQuery();

                return Ok(new
                {
                    Success = true,
                    Message = "Saved Successfully"
                });
            
          
        }
    }

    public class InternalShiftingSaveRequest
    {
        public DateTime SubmitDate { get; set; }

        public List<InternalShiftingRow> Rows { get; set; } = new();
    }

    public class InternalShiftingRow
    {
        public int BillNo { get; set; }
        public decimal? Payment { get; set; }
        public decimal? isTDS { get; set; }
        public decimal? TDS { get; set; }
        public decimal? GST { get; set; }
        public decimal? Shortage { get; set; }
        public string? Remarks { get; set; } 
        public DateTime? PassDate { get; set; }
        public int ExtShortage { get; set; }


        public string? Barcode { get; set; }
    }
}
