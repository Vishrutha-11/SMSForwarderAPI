using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Payments.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JVMController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public JVMController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        public IActionResult GetOtherPPDShipmentDetailsForPPDGCReceive(GetOtherPPDShipmentForPPDGCReceiveModel model)
        {
            try
            {
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SqlConnection")))
                {
                    using (SqlCommand cmd = new SqlCommand("GetOtherPPDShipmentDetailsForPPDGCReceive", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ShipmentNo", (object?)model.ShipmentNo ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@InvoiceNo", (object?)model.InvoiceNo ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@GcNo", (object?)model.GcNo ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@CustomerId", (object?)model.CustomerId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Year", (object?)model.Year ?? DBNull.Value);
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                }
                return Ok(ConvertDataTable(dt));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("OtherPPDGcRecivedAtLocation")]
        public IActionResult OtherPPDGcRecivedAtLocation(OtherPPDGcRecivedAtLocationModel model)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SqlConnection")))
                {
                    using (SqlCommand cmd = new SqlCommand("OtherPPDGcReceiveAtLocation", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ShipMentID", model.ShipMentID);
                        cmd.Parameters.AddWithValue("@Weighment", model.Weighment);
                        cmd.Parameters.AddWithValue("@DeliveryDate", model.DeliveryDate);
                        cmd.Parameters.AddWithValue("@Location", (object?)model.Location ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@RecivedBy", (object?)model.RecivedBy ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Remarks", (object?)model.Remarks ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@UnLoad", (object?)model.UnLoad ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@HSM3", (object?)model.HSM3 ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ExpenseUnloadId", model.ExpenseUnloadId);
                        cmd.Parameters.AddWithValue("@cb", model.Cb);
                        con.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            return Ok(new { success = true, message = "Saved Successfully" });
                        }
                        else
                        {
                            return Ok(new { success = false, message = "No rows affected" });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
        private List<Dictionary<string, object?>> ConvertDataTable(DataTable dt)
        {
            var result = new List<Dictionary<string, object?>>();

            foreach (DataRow row in dt.Rows)
            {
                var dict = new Dictionary<string, object?>();

                foreach (DataColumn col in dt.Columns)
                {
                    dict[col.ColumnName] = row[col] == DBNull.Value ? null : row[col];
                }

                result.Add(dict);
            }

            return result;
        }
    }

    public class GetOtherPPDShipmentForPPDGCReceiveModel
    {
        public string? ShipmentNo { get; set; }
        public string? InvoiceNo { get; set; }
        public string? GcNo { get; set; }
        public int? CustomerId { get; set; }
        public int? Year { get; set; }
    }

    public class OtherPPDGcRecivedAtLocationModel
    {
        public int ShipMentID { get; set; }
        public decimal Weighment { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string? Location { get; set; }
        public string? RecivedBy { get; set; }
        public string? Remarks { get; set; }
        public string? UnLoad { get; set; }
        public string? HSM3 { get; set; }
        public int ExpenseUnloadId { get; set; }
        public int Cb { get; set; }
    }
}
