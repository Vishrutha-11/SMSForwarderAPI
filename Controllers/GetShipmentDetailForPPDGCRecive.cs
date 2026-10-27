using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

using System.Data;

namespace Payments.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GetShipmentDetailForPPDGCReciveController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public GetShipmentDetailForPPDGCReciveController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        public IActionResult GetShipmentDetailForPPDGCRecive(GetShipmentForPPDGCReciveModel model)
        {
            try
            {
                DataTable dt = new DataTable();

                using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SqlConnection")))
                {
                    using (SqlCommand cmd = new SqlCommand("GetShipmentDetailForPPDGCRecive", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@ShipmentNo", (object?)model.ShipmentNo ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@InvoiceNo", (object?)model.InvoiceNo ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@GcNo", (object?)model.GcNo ?? DBNull.Value);

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
      
  
        [HttpPost("PPDGCReciveAtLocation")]
        public IActionResult PPDGcRecivedAtLocation(
            [FromBody] PPDGCReceiveRequest request)
        {

            try
            {
                using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SqlConnection")))
                {

                    using (SqlCommand cmd = new SqlCommand(
                        "PPDGcRecivedAtLocation",
                        con))
                    {

                        cmd.CommandType =
                            CommandType.StoredProcedure;


                        cmd.Parameters.AddWithValue(
                            "@ShipMentID",
                            request.ShipMentID);


                        cmd.Parameters.AddWithValue(
                            "@Weighment",
                            request.Weighment);


                        cmd.Parameters.AddWithValue(
                            "@DeliveryDate",
                            request.DeliveryDate);


                        cmd.Parameters.AddWithValue(
                            "@Location",
                            request.Location ?? "");


                        cmd.Parameters.AddWithValue(
                            "@RecivedBy",
                            request.RecivedBy ?? "");

                        cmd.Parameters.AddWithValue(
                            "@Remarks",
                            string.IsNullOrWhiteSpace(request.Remarks)
                             ? ""
                            : request.Remarks);

                        cmd.Parameters.AddWithValue(
                            "@UnLoad",
                            request.UnLoad);

                        con.Open();

                        cmd.ExecuteNonQuery();

                    }

                }
                return Ok(new
                {
                    success = true,
                    message = "PPD GC Receive saved successfully"
                });

            }
            catch (Exception ex)
            {

                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });

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


    public class GetShipmentForPPDGCReciveModel
    {
        public string? ShipmentNo { get; set; }
        public string? InvoiceNo { get; set; }
        public string? GcNo { get; set; }
 
    }
     public class PPDGCReceiveRequest
        {
            public int ShipMentID { get; set; }
            public decimal Weighment { get; set; }
            public DateTime DeliveryDate { get; set; }
            public string Location { get; set; }
            public string RecivedBy { get; set; }
            public int UnLoad { get; set; }
        public string Remarks { get; set; }
    }

    
}