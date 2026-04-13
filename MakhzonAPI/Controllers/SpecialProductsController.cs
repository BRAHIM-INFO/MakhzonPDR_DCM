using MakhzonAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace MakhzonAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class SpecialProductsController : ControllerBase
    {
        private string connString = "Data Source=SQL5113.site4now.net;Initial Catalog=db_ac7979_makhzondb;User Id=db_ac7979_makhzondb_admin;Password=Bahi.2026;Encrypt=True;TrustServerCertificate=True;";

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = new List<SpecialProduct>();
            using (var conn = new SqlConnection(connString))
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand("SELECT * FROM SpecialProducts ORDER BY Id DESC", conn);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        list.Add(new SpecialProduct
                        {
                            Id = (int)reader["Id"],
                            REF = reader["REF"].ToString(),
                            INTITULE = reader["INTITULE"].ToString(),
                            MACHINE = reader["MACHINE"].ToString(),
                            QTE = (int)reader["QTE"],
                            CASIER = reader["CASIER"].ToString()
                        });
                    }
                }
            }
            return Ok(list);
        }

        [HttpPost]
        public async Task<IActionResult> Add(SpecialProduct p)
        {
            using (var conn = new SqlConnection(connString))
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand("INSERT INTO SpecialProducts (REF, INTITULE, MACHINE, QTE, CASIER) VALUES (@ref, @int, @mac, @qte, @cas)", conn);
                cmd.Parameters.AddWithValue("@ref", p.REF);
                cmd.Parameters.AddWithValue("@int", p.INTITULE);
                cmd.Parameters.AddWithValue("@mac", p.MACHINE);
                cmd.Parameters.AddWithValue("@qte", p.QTE);
                cmd.Parameters.AddWithValue("@cas", p.CASIER);
                await cmd.ExecuteNonQueryAsync();
            }
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            using (var conn = new SqlConnection(connString))
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand("DELETE FROM SpecialProducts WHERE Id = @id", conn);
                cmd.Parameters.AddWithValue("@id", id);
                await cmd.ExecuteNonQueryAsync();
            }
            return Ok();
        }
        // 1. التعديل (Update)
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, SpecialProduct p)
        {
            using (var conn = new SqlConnection(connString))
            {
                await conn.OpenAsync();
                var sql = "UPDATE SpecialProducts SET REF=@ref, INTITULE=@int, MACHINE=@mac, QTE=@qte, CASIER=@cas WHERE Id=@id";
                var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@ref", p.REF);
                cmd.Parameters.AddWithValue("@int", p.INTITULE);
                cmd.Parameters.AddWithValue("@mac", p.MACHINE);
                cmd.Parameters.AddWithValue("@qte", p.QTE);
                cmd.Parameters.AddWithValue("@cas", p.CASIER);
                await cmd.ExecuteNonQueryAsync();
            }
            return Ok();
        }
         
    }
}
