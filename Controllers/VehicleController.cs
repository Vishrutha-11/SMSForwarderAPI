using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using Microsoft.Data.SqlClient;

namespace Payments.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class VehicleController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly string _gpsConnectionString;

        public VehicleController(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("SqlConnection");
            _gpsConnectionString = config.GetConnectionString("GPSTrackConnection");
        }

        [HttpGet("GetVehicles")]
        public async Task<IActionResult> GetOwnVehicles()
        {
            var result = new List<OwnVehicleDto>();

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("GetOwnVehicle", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                await conn.OpenAsync();

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        result.Add(new OwnVehicleDto
                        {
                            VehicleId = reader.GetInt32(reader.GetOrdinal("VehicleID")),
                            VehicleNo = reader.GetString(reader.GetOrdinal("VehicleNo"))
                        });
                    }
                }
            }

            return Ok(result);
        }
        // GET api/Vehicle/GetVehicleInsurances               -> all records
        // GET api/Vehicle/GetVehicleInsurances?vehicleId=5   -> records of one vehicle
        [HttpGet("GetVehicleInsurances")]
        public async Task<IActionResult> GetVehicleInsurances([FromQuery] int? vehicleId)
        {
            var list = await ReadInsurances();
            if (vehicleId.HasValue)
                list = list.Where(x => x.VehicleID == vehicleId.Value).ToList();
            return Ok(list);
        }

        // GET api/Vehicle/GetVehicleInsurance/12 -> one record (for edit prefill)
        [HttpGet("GetVehicleInsurance/{id:int}")]
        public async Task<IActionResult> GetVehicleInsurance(int id)
        {
            var item = (await ReadInsurances()).FirstOrDefault(x => x.VehicleInsuranceID == id);
            if (item == null)
                return NotFound($"No insurance record found with ID {id}.");
            return Ok(item);
        }

        private async Task<List<VehicleInsuranceListDto>> ReadInsurances()
        {
            var list = new List<VehicleInsuranceListDto>();

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("GetVehicleInsurance", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                await conn.OpenAsync();

                using (var r = await cmd.ExecuteReaderAsync())
                {
                    while (await r.ReadAsync())
                    {
                        list.Add(new VehicleInsuranceListDto
                        {
                            VehicleInsuranceID = Convert.ToInt32(r["VehicleInsuranceID"]),
                            SlNo = Convert.ToInt32(r["SL No"]),
                            VehicleID = Convert.ToInt32(r["VehicleID"]),
                            VehicleNo = Str(r, "VehicleNo"),
                            PremiumAmount = Dec(r, "PremiumAmount"),
                            NCB = Str(r, "NCB"),
                            PolicyNo = Str(r, "PolicyNo"),
                            IDVValue = Dec(r, "IDVValue"),
                            ServiceTaxAmount = Dec(r, "ServiceTaxAmount"),
                            CustomerID = Str(r, "CustomerID"),
                            InsuranceCompanyName = Str(r, "InsuranceCompanyName"),
                            GVW = Dec(r, "GVW"),
                            FromInsuranceDate = Dt(r, "FromInsuranceDate"),
                            ToInsuranceDate = Dt(r, "ToInsuranceDate"),
                            Agent = Str(r, "Agent"),
                            CashBack = Dec(r, "CashBack"),
                            Owner = Str(r, "Owner"),
                            Remarks = Str(r, "Remarks")
                        });
                    }
                }
            }
            return list;
        }

        // Null-safe readers (any optional column can be NULL, whatever its SQL type)
        private static string? Str(SqlDataReader r, string col)
        {
            int i = r.GetOrdinal(col);
            return r.IsDBNull(i) ? null : Convert.ToString(r.GetValue(i));
        }
        private static decimal? Dec(SqlDataReader r, string col)
        {
            int i = r.GetOrdinal(col);
            return r.IsDBNull(i) ? null : Convert.ToDecimal(r.GetValue(i));
        }
        private static DateTime? Dt(SqlDataReader r, string col)
        {
            int i = r.GetOrdinal(col);
            return r.IsDBNull(i) ? null : Convert.ToDateTime(r.GetValue(i));
        }
        [HttpPost("InsertVehicleInsurance")]
        public async Task<IActionResult> InsertVehicleInsurance([FromBody] VehicleInsuranceDto dto)
        {
            int rowsAffected;

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("InsertVehicleInsurance", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                AddInsuranceParameters(cmd, dto);

                await conn.OpenAsync();
                rowsAffected = await cmd.ExecuteNonQueryAsync();
            }

            if (rowsAffected == 0)
                return Conflict("An overlapping insurance record already exists for this vehicle in the given date range.");

            return Ok(new { message = "Vehicle insurance record inserted successfully." });
        }

        [HttpPost("UpdateVehicleInsurance")]
        public async Task<IActionResult> UpdateVehicleInsurance([FromBody] VehicleInsuranceUpdateDto dto)
        {
            int rowsAffected;

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("UpdateVehicleInsurance", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@VehicleInsuranceID", dto.VehicleInsuranceID);
                AddInsuranceParameters(cmd, dto);

                await conn.OpenAsync();
                rowsAffected = await cmd.ExecuteNonQueryAsync();
            }

            if (rowsAffected == 0)
                return NotFound($"No insurance record found with ID {dto.VehicleInsuranceID}.");

            return Ok(new { message = "Vehicle insurance record updated successfully." });
        }

        // Shared by insert and update. Every optional value becomes DBNull when it is null,
        // because AddWithValue with a plain C# null omits the parameter and the procedure then fails.
        private static void AddInsuranceParameters(SqlCommand cmd, VehicleInsuranceDto dto)
        {
            cmd.Parameters.AddWithValue("@VehicleID", dto.VehicleID);
            cmd.Parameters.AddWithValue("@PremiumAmount", (object?)dto.PremiumAmount ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@NCB", (object?)dto.NCB ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PolicyNo", (object?)dto.PolicyNo ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IDVValue", (object?)dto.IDVValue ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ServiceTaxAmount", (object?)dto.ServiceTaxAmount ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@CustomerID", (object?)dto.CustomerID ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@InsuranceCompanyName", (object?)dto.InsuranceCompanyName ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@GVW", (object?)dto.GVW ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@FromInsuranceDate", (object?)dto.FromInsuranceDate ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ToInsuranceDate", (object?)dto.ToInsuranceDate ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Agent", (object?)dto.Agent ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@CashBack", (object?)dto.CashBack ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Owner", (object?)dto.Owner ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Remarks", (object?)dto.Remarks ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Cb", dto.Cb);
        }



        [HttpGet("GetIntangleVehicleIDs")]
        public async Task<IActionResult> GetIntangleVehicleIDs([FromQuery] string Vehicle)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_gpsConnectionString))
                {
                    return BadRequest("GPSTrackConnection connection string not configured.");
                }

                var result = new List<object>();

                using (var conn = new SqlConnection(_gpsConnectionString))
                using (var cmd = new SqlCommand("getIntangleVehicleIDs", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@Vehicle", SqlDbType.VarChar).Value =
                        string.IsNullOrWhiteSpace(Vehicle)
                            ? DBNull.Value
                            : Vehicle;

                    await conn.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            result.Add(new
                            {
                                VehicleId = reader["VehicleId"],
                                Vehicle = reader["Vehicle"]
                            });
                        }
                    }
                }

                if (result.Count == 0)
                {
                    return NotFound($"No Intangle vehicle found for {Vehicle}.");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }

    public class OwnVehicleDto
    {
        public int VehicleId { get; set; }
        public string VehicleNo { get; set; }
    }

    // Only VehicleID and Cb are mandatory. Everything else is optional.
    public class VehicleInsuranceDto
    {
        public int VehicleID { get; set; }
        public decimal? PremiumAmount { get; set; }
        public string? NCB { get; set; }
        public string? PolicyNo { get; set; }
        public decimal? IDVValue { get; set; }
        public decimal? ServiceTaxAmount { get; set; }
        public string? CustomerID { get; set; }
        public string? InsuranceCompanyName { get; set; }
        public decimal? GVW { get; set; }
        public DateTime? FromInsuranceDate { get; set; }
        public DateTime? ToInsuranceDate { get; set; }
        public string? Agent { get; set; }
        public decimal? CashBack { get; set; }
        public string? Owner { get; set; }
        public string? Remarks { get; set; }
        public int Cb { get; set; }
    }

    // Same fields plus the record id, so the shared parameter helper works for both.
    public class VehicleInsuranceUpdateDto : VehicleInsuranceDto
    {
        public int VehicleInsuranceID { get; set; }
    }
    public class VehicleInsuranceListDto
    {
        public int VehicleInsuranceID { get; set; }
        public int SlNo { get; set; }
        public int VehicleID { get; set; }
        public string? VehicleNo { get; set; }
        public decimal? PremiumAmount { get; set; }
        public string? NCB { get; set; }
        public string? PolicyNo { get; set; }
        public decimal? IDVValue { get; set; }
        public decimal? ServiceTaxAmount { get; set; }
        public string? CustomerID { get; set; }
        public string? InsuranceCompanyName { get; set; }
        public decimal? GVW { get; set; }
        public DateTime? FromInsuranceDate { get; set; }
        public DateTime? ToInsuranceDate { get; set; }
        public string? Agent { get; set; }
        public decimal? CashBack { get; set; }
        public string? Owner { get; set; }
        public string? Remarks { get; set; }
    }
}