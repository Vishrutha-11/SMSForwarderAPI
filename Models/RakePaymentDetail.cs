//namespace PaymentDetails.Models
//{
//    public class RakePaymentDetail
//    {
//        public int SlNo { get; set; }

//        public string BillNo { get; set; }

//        public string Place { get; set; }

//        public DateTime? PassDate { get; set; }

//        public string Barcode { get; set; }

//        public DateTime? MonthYear { get; set; }

//        public DateTime? ActualSubmitDate { get; set; }

//        public string RakeNo { get; set; }

//        public string RakeTypeName { get; set; }

//        public decimal Tonns { get; set; }

//        public decimal Amount { get; set; }

//        public decimal Payment { get; set; }

//        public int IsTDS { get; set; }

//        public decimal TDS { get; set; }

//        public int GST { get; set; }

//        public decimal ExtShortage { get; set; }

//        public decimal Shortage { get; set; }

//        public string Remarks { get; set; }

//        // Required for Update API
//        public DateTime? SubmitDate { get; set; }

//        public int CustomerId { get; set; }

//        public int UB { get; set; }

//        public string Path { get; set; }
//    }
//}


using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using Payments.Controllers;
using System.Data;

namespace PaymentDetails.Models
{
    public class RakePaymentDetail
    {
        public int SlNo { get; set; }

        public int BillNo { get; set; }

        public string Place { get; set; } = "";

        public DateTime? PassDate { get; set; }

        public string Barcode { get; set; } = "";

        public DateTime? MonthYear { get; set; }

        public DateTime? ActualSubmitDate { get; set; }

        public string RakeNo { get; set; } = "";

        public string RakeTypeName { get; set; } = "";

       
        public decimal Tonns { get; set; }

        public decimal Amount { get; set; }

        public decimal Payment { get; set; }

        public int IsTDS { get; set; }

        public int TDS { get; set; }

        public int GST { get; set; }

        public decimal ExtShortage { get; set; }

        public decimal Shortage { get; set; }

        public string Remarks { get; set; } = "";

        public int? SubmitDate { get; set; }
        public int? SubmitMonth { get; set; }
        public int? SubmitYear { get; set; }

        public int CustomerId { get; set; }
        public int UB { get; set; }
    }
}





//[HttpPost("GetInternalShiftingBillID")]
//public string GetInternalShiftingBillID(
//[FromBody] InternalShiftingSaveRequest request)
//{
//    SqlConnection con = new SqlConnection(connectionString);
//    SqlCommand cmd = new SqlCommand("GetInternalShiftingBillID", con);

//    DateTime submitDate = Convert.ToDateTime(request.SubmitDate);

//    cmd.CommandType = CommandType.StoredProcedure;

//    cmd.Parameters.AddWithValue("@SubmitDate", submitDate.Day);
//    cmd.Parameters.AddWithValue("@SubmitMonth", submitDate.Month);
//    cmd.Parameters.AddWithValue("@SubmitYear", submitDate.Year);

//    con.Open();

//    SqlDataReader reader = cmd.ExecuteReader();

//    DataTable dataTable = new DataTable { TableName = "InternalShiftingPaymentDetails" };
//    dataTable.Load(reader);

//    con.Close();

//    string JSONresult = JsonConvert.SerializeObject(dataTable);



//    return JSONresult;
//}


        //   [HttpPost("GetInternalShiftingBillID")]
        //   public IActionResult GetInternalShiftingBillID(
        //[FromQuery] int submitDate,
        //[FromQuery] int submitMonth,
        //[FromQuery] int submitYear)
        //   {
        //       using SqlConnection con =
        //           new SqlConnection(connectionString);

        //       using SqlCommand cmd =
        //           new SqlCommand("GetInternalShiftingBillID", con);

        //       cmd.CommandType = CommandType.StoredProcedure;

        //       cmd.Parameters.AddWithValue("@SubmitDate", submitDate);
        //       cmd.Parameters.AddWithValue("@SubmitMonth", submitMonth);
        //       cmd.Parameters.AddWithValue("@SubmitYear", submitYear);

        //       con.Open();

        //       SqlDataAdapter da =
        //           new SqlDataAdapter(cmd);

        //       DataSet ds =
        //           new DataSet();

        //       da.Fill(ds);

        //       var paymentDetails =
        //           ds.Tables[0].AsEnumerable()
        //           .Select(row => new
        //           {
        //               SlNo = row["SlNo"] == DBNull.Value ? null : row["SlNo"],
        //               BillNo = row["BillNo"] == DBNull.Value ? null : row["BillNo"],
        //               Place = row["Place"] == DBNull.Value ? null : row["Place"],
        //               Tonns = row["Tonns"] == DBNull.Value ? null : row["Tonns"],

        //               Amount = row["Amount"] == DBNull.Value? null: row["Amount"],

        //               Payment = row["Payment"] == DBNull.Value
        //                   ? null
        //                   : row["Payment"],

        //               IsTDS = row["IsTDS"] == DBNull.Value? null: row["IsTDS"],

        //               TDS = row["TDS"] == DBNull.Value
        //                   ? null
        //                   : row["TDS"],

        //               GST = row["GST"] == DBNull.Value
        //                   ? null
        //                   : row["GST"],

        //               Shortage = row["Shortage"] == DBNull.Value
        //                   ? null
        //                   : row["Shortage"],

        //               Remarks = row["Remarks"] == DBNull.Value
        //                   ? null
        //                   : row["Remarks"],

        //               Passdate = row["Passdate"] == DBNull.Value
        //                   ? null
        //                   : Convert.ToDateTime(row["Passdate"])
        //                       .ToString("yyyy-MM-dd"),

        //               ExtShortage = row["ExtShortage"] == DBNull.Value
        //                   ? null
        //                   : row["ExtShortage"],

        //               Barcode = row["Barcode"] == DBNull.Value
        //                   ? null
        //                   : row["Barcode"]
        //           }); 


        //       return Ok(new
        //       {
        //           PaymentDetails = paymentDetails,
        //           //ShortageDetails = shortageDetails
        //       });
        //   }