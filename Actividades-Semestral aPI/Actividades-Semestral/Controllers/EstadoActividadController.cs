using Actividades_Semestral.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Actividades_Semestral.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EstadoActividadController : ControllerBase
    {
        private readonly GestorActividadesContext _context;

        // Constructor del controlador
        public EstadoActividadController(GestorActividadesContext context)
        {
            _context = context;
        }

        // Endpoint para obtener los estados
        [HttpGet]
        public async Task<IActionResult> GetEstados()
        {
            try
            {
                var estadosActividades = await _context.EstadoActividades
                    .Where(e => e.IdTipoEstado == 1) // 1 para estados de actividades
                    .Select(e => new { e.IdEstado, e.NombreEstado })
                    .ToListAsync();
                return Ok(new
                {
                    estadosActividades
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al procesar la solicitud", details = ex.Message });
            }
        }
        [HttpGet("estado/{idEstado?}")]
        public async Task<IActionResult> GetByEstado(int? idEstado)
        {
            try
            {
                IQueryable<Actividade> query = _context.Actividades
                    .Include(a => a.IdEstadoNavigation); // Solo incluimos el estado de la actividad

                // Filtrar por estado si se proporciona
                if (idEstado.HasValue)
                {
                    query = query.Where(a => a.IdEstadoNavigation.IdEstado == idEstado);
                }

                // Proyección de las propiedades relevantes
                var actividades = await query.Select(a => new
                {
                    a.IdActividad,
                    NombreActividad = a.Nombre,
                    Fecha = a.Fecha,
                    Estado = a.IdEstadoNavigation != null ? a.IdEstadoNavigation.NombreEstado : "Sin estado",
                    CuposMaximos = a.LimiteCupos,
                    CuposOcupados = a.Inscripciones.Count
                }).ToListAsync();

                if (!actividades.Any())
                {
                    return Ok(new List<object>()); // Devolver una lista vacía si no hay actividades
                }

                return Ok(actividades);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al procesar la solicitud", details = ex.Message });
            }
        }


    }
}
