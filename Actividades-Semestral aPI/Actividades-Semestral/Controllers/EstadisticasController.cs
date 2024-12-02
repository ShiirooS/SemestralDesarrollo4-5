using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using System.Data;

namespace Actividades_Semestral.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EstadisticasController : ControllerBase
    {
        private readonly string _connectionString;

        public EstadisticasController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("conexion")!;
        }

        [HttpGet("estadisticas")]
        public async Task<IActionResult> GetEstadisticas([FromQuery] int idUsuario)
        {
            try
            {
                // Obtener idUsuario del encabezado o sesión
                if (string.IsNullOrEmpty(HttpContext.Request.Headers["idUsuario"].FirstOrDefault()) || !int.TryParse(HttpContext.Request.Headers["idUsuario"].FirstOrDefault(), out int parsedIdUsuario))
                {
                    return Unauthorized(new { mensaje = "Usuario no autenticado." });
                }



                using var connection = new MySqlConnection(_connectionString);
                await connection.OpenAsync();
                var query = "CALL GetEstadisticasUsuario(@idUsuario)";
                using var command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@idUsuario", parsedIdUsuario);

                using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return Ok(new
                    {
                        idUsuario = reader["id_usuario"],
                        actividadesInscritas = reader["actividades_inscritas"],
                        actividadesOrganizadas = reader["actividades_organizadas"],
                        actividadesEnEspera = reader["actividades_en_espera"],
                        actividadesAceptadas = reader["actividades_aceptadas"],
                        actividadesRechazadas = reader["actividades_rechazadas"]
                    });
                }

                return NotFound(new { mensaje = "No se encontraron estadísticas para este usuario." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error al obtener estadísticas", detalles = ex.Message });
            }
        }
    }
}