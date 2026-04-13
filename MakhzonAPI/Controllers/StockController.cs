using MakhzonAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using MySql.Data.MySqlClient; 
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace MakhzonAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockController : ControllerBase
    {

        // سلسلة الاتصال الأصلية الخاصة بك لـ MSSQL
        private string connString = "Data Source=SQL5113.site4now.net;Initial Catalog=db_ac7979_makhzondb;User Id=db_ac7979_makhzondb_admin;Password=Bahi.2026;Encrypt=True;TrustServerCertificate=True;";
          
        [HttpGet]
        public async Task<IActionResult> GetStock(string search = "")
        {
            var list = new List<Stock>();
            using (var conn = new SqlConnection(connString))
            {
                await conn.OpenAsync();
                // أضفنا LastUpdated في الاستعلام
                string sql = string.IsNullOrEmpty(search)
                    ? "SELECT TOP 100 REF, INTITULE, QTE, en_Stock,  CASIER, LastUpdated FROM dbo.StockItems ORDER BY REF"
                    : "SELECT TOP 500 REF, INTITULE, QTE, en_Stock,  CASIER, LastUpdated FROM dbo.StockItems WHERE REF LIKE @q OR INTITULE LIKE @q ORDER BY REF";

                var cmd = new SqlCommand(sql, conn);
                if (!string.IsNullOrEmpty(search)) cmd.Parameters.AddWithValue("@q", "%" + search + "%");

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        list.Add(new Stock
                        {
                            REF = reader["REF"]?.ToString(),
                            INTITULE = reader["INTITULE"]?.ToString(),
                            QTE = Convert.ToDouble(reader["QTE"]),
                            // 2. قراءة en_Stock مع معالجة القيم الفارغة (مهم جداً لعدم توقف الكود)
                            en_Stock = reader["en_Stock"] != DBNull.Value ? Convert.ToInt32(reader["en_Stock"]) : 0,


                            CASIER = reader["CASIER"]?.ToString(),
                            // معالجة التاريخ إذا كان فارغاً في قاعدة البيانات
                            LastUpdated = reader["LastUpdated"] != DBNull.Value ? (DateTime)reader["LastUpdated"] : (DateTime?)null
                        });
                    }
                }
            }
            return Ok(list);
        } 


        [HttpPut("UpdateCasier")]
        public async Task<IActionResult> UpdateCasier([FromQuery] string refId, [FromQuery] string newCasier, [FromQuery] int newenStock)
        {

            try
            {
                using (var conn = new SqlConnection(connString))
                {
                    await conn.OpenAsync();
                    string tableName = "dbo.StockItems";

                    // قمنا بإضافة LastUpdated = GETDATE() لتحديث التاريخ تلقائياً
                    string sql = $"UPDATE {tableName} SET CASIER = @casier,en_Stock = @enstock, LastUpdated = GETDATE() WHERE REF = @ref";

                    var cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@casier", (object)newCasier ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@enstock", (object)newenStock ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ref", refId);

                    int rowsAffected = await cmd.ExecuteNonQueryAsync();
                     

                    if (rowsAffected > 0)
                        return Ok(new { message = "Update Success" });
                    else
                        return NotFound($"Reference {refId} not found.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Database Error: " + ex.Message);
            }
        }


        [HttpGet("stats")]
        public async Task<IActionResult> GetDashboardStats()
        {
            try
            {
                using (var conn = new SqlConnection(connString))
                {
                    await conn.OpenAsync();

                    // 1. إجمالي المنتجات في القائمة الرئيسية
                    var cmdTotal = new SqlCommand("SELECT COUNT(*) FROM dbo.StockItems", conn);
                    int totalStock = (int)await cmdTotal.ExecuteScalarAsync();

                    // 2. المنتجات التي تم تعديلها (LastUpdated ليس فارغاً)
                    var cmdUpdated = new SqlCommand("SELECT COUNT(*) FROM dbo.StockItems WHERE LastUpdated IS NOT NULL", conn);
                    int updatedStock = (int)await cmdUpdated.ExecuteScalarAsync();

                    // 3. المنتجات الجديدة (من الجدول الخاص SpecialProducts)
                    var cmdNew = new SqlCommand("SELECT COUNT(*) FROM dbo.SpecialProducts", conn);
                    int newProducts = (int)await cmdNew.ExecuteScalarAsync();

                    return Ok(new
                    {
                        Total = totalStock,
                        Updated = updatedStock,
                        New = newProducts
                    });
                }
            }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }
          
    }
}
