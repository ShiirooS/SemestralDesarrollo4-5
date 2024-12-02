using Actividades_Semestral.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Actividades_Semestral.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistroActividadesController : ControllerBase  //versiona Actualizada
    {
        private readonly GestorActividadesContext _context;

        public RegistroActividadesController(GestorActividadesContext context)
        {
            _context = context;
        }

        // GET: api/RegistroActividades
        [HttpGet]
        public async Task<IActionResult> ObtenerHistorial(string estado, int idUsuario)
        {
            try
            {
                // Verificar si el usuario es estudiante
                var usuario = await _context.Usuarios
                    .FirstOrDefaultAsync(p => p.IdUsuario == idUsuario && p.IdRol == 2);

                if (usuario == null)
                {
                    return NotFound(new { message = "Usuario no encontrado o no es estudiante." });
                }

                // Ejecutar procedimientos almacenados para actualizar los estados
                await _context.Database.ExecuteSqlInterpolatedAsync(
                    $"CALL EstadosInscripciones({usuario.IdUsuario}, null)");

                if (estado == "todas")
                {
                    // Propuestas de actividades
                    var actividadesPropuestas = await _context.PropuestasActividades
                        .Where(p => p.IdUsuario == usuario.IdUsuario) // Solo propuestas del usuario
                        .Include(p => p.IdEstadoNavigation)
                        .Select(p => new
                        {
                            IdActividad = (int?)null,
                            Nombre = p.Nombre,
                            Estado = p.IdEstadoNavigation.NombreEstado, // Refleja el estado actual
                            Descripcion = p.Descripcion,
                            FechaInscripcion = (DateTime?)null,
                            ComentarioRechazo = p.ComentarioRechazo
                        })
                        .ToListAsync();

                    // Inscripciones
                    var actividadesInscritas = await _context.Inscripciones
                        .Include(i => i.IdActividadNavigation)
                        .Include(i => i.IdEstadoNavigation)
                        .Where(i => i.IdUsuario == usuario.IdUsuario)
                        .Select(i => new
                        {
                            IdActividad = (int?)i.IdActividadNavigation.IdActividad,
                            Nombre = i.IdActividadNavigation.Nombre,
                            Estado = i.IdEstadoNavigation.NombreEstado,
                            Descripcion = i.IdActividadNavigation.Descripcion,
                            FechaInscripcion = i.FechaInscripcion,
                            ComentarioRechazo = (string?)null

                        })
                        .ToListAsync();

                    // Combinar las actividades
                    var todasLasActividades = actividadesPropuestas.Concat(actividadesInscritas).ToList();

                    return Ok(todasLasActividades);
                }
                else if (estado == "propuestas")
                {
                    var actividades = await _context.PropuestasActividades
                         .Where(p => p.IdUsuario == usuario.IdUsuario)
                        .Include(p => p.IdEstadoNavigation) 
                        .Select(p => new
                        {
                            Nombre = p.Nombre,
                            Estado = p.IdEstadoNavigation.NombreEstado, // Refleja el estado actual, cambio realizado
                            Descripcion = p.Descripcion,
                            FechaInscripcion = (DateTime?)null,
                            ComentarioRechazo = p.ComentarioRechazo
                        })
                        .ToListAsync();

                    return Ok(actividades);
                }
                else if (estado == "inscritas")
                {
                    var actividadesInscritas = await _context.Inscripciones
                        .Include(i => i.IdActividadNavigation)
                        .Include(i => i.IdEstadoNavigation)
                        .Where(i => i.IdUsuario == usuario.IdUsuario)
                        .Select(i => new
                        {
                            IdActividad = i.IdActividadNavigation.IdActividad,
                            Nombre = i.IdActividadNavigation.Nombre,
                            Estado = i.IdEstadoNavigation.NombreEstado,
                            Descripcion = i.IdActividadNavigation.Descripcion,
                            FechaInscripcion = i.FechaInscripcion,
                            ComentarioRechazo = (string?)null
                        })
                        .ToListAsync();

                    return Ok(actividadesInscritas);
                }
                else
                {
                    return BadRequest(new { message = "Estado no válido." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error al obtener el registro de las actividades.",
                    detail = ex.Message
                });
            }
        }

        // GET: api/RegistroActividades/5
        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerActividadPorId(int id)
        {
            try
            {
                var actividad = await _context.PropuestasActividades
                    .Include(p => p.IdEstadoNavigation) // Relación con estado de las actividades
                    .Where(p => p.IdPropuesta == id)
                    .Select(p => new
                    {
                        Nombre = p.Nombre,
                        Estado = p.IdEstadoNavigation.NombreEstado, // Nombre del estado
                        Descripcion = p.Descripcion
                    })
                    .FirstOrDefaultAsync();

                if (actividad == null)
                {
                    return NotFound(new { message = "La actividad no existe." });
                }

                return Ok(actividad);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error al obtener la actividad.",
                    detail = ex.Message
                });
            }
        }

    }

}
