using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Actividades_Semestral.Models;

namespace Actividades_Semestral.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InscripcionesController : ControllerBase
    {
        private readonly GestorActividadesContext _context;

        public InscripcionesController(GestorActividadesContext context)
        {
            _context = context;
        }
        //--------------------------------------------------------------------------------------

        [HttpGet]
        [Route("estado")]
        public async Task<ActionResult> VerificarEstado(int idActividad, string correo)
        {
            if (string.IsNullOrEmpty(correo) || idActividad <= 0)
            {
                return BadRequest(new { message = "Datos inválidos." });
            }

            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Correo == correo);
            if (usuario == null)
            {
                return NotFound(new { message = "Usuario no encontrado." });
            }

            var inscrito = await _context.Inscripciones
                .AnyAsync(i => i.IdUsuario == usuario.IdUsuario && i.IdActividad == idActividad);

            return Ok(new { inscrito });
        }






        //--------------------------------------------------------------------------------------
        [HttpDelete]
        [Route("cancelar")]
        public async Task<ActionResult> CancelarInscripcion([FromBody] SolicitarInscripcion request)
        {
            if (string.IsNullOrEmpty(request.Correo) || request.IdActividad <= 0)
            {
                return BadRequest(new { message = "Datos de cancelación inválidos." });
            }

            // Buscar usuario
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Correo == request.Correo);
            if (usuario == null)
            {
                return NotFound(new { message = "Usuario no encontrado." });
            }

            // Buscar inscripción
            var inscripcion = await _context.Inscripciones
                .FirstOrDefaultAsync(i => i.IdUsuario == usuario.IdUsuario && i.IdActividad == request.IdActividad);
            if (inscripcion == null)
            {
                return NotFound(new { message = "No estás inscrito en esta actividad." });
            }

            // Eliminar inscripción y actualizar cupos
            _context.Inscripciones.Remove(inscripcion);
            var actividad = await _context.Actividades.FirstOrDefaultAsync(a => a.IdActividad == request.IdActividad);
            if (actividad != null)
            {
                actividad.LimiteCupos++;
                _context.Entry(actividad).State = EntityState.Modified;
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "Inscripción cancelada con éxito." });
        }

        // ------------------------------------------------------------------------------------------------

        [HttpPost]
        [Route("registrar")]
        public async Task<ActionResult> RegistrarActividad([FromBody] SolicitarInscripcion request)
        {
            if (string.IsNullOrEmpty(request.Correo) || request.IdActividad <= 0)
            {
                return BadRequest(new { message = "Datos de inscripción inválidos." });
            }

            // Busca al usuario por su correo POR SU CORREOOOO
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Correo == request.Correo);
            if (usuario == null)
            {
                return NotFound(new { message = "Usuario no encontrado." });
            }

            // Buscar la actividad poke si no nonas nonas
            var actividad = await _context.Actividades.FirstOrDefaultAsync(a => a.IdActividad == request.IdActividad);
            if (actividad == null)
            {
                return NotFound(new { message = "Actividad no encontrada." });
            }
            // Validar si el estado de la actividad es inválido
           
            if (actividad.IdEstado == 2)
            {
                return BadRequest(new { message = "No puedes inscribirte a esta actividad porque ya está finalizada." });
            }
            else if (actividad.IdEstado == 3)
            {
                return BadRequest(new { message = "No puedes inscribirte a esta actividad porque ha sido cancelada." });
            }
            else if (actividad.IdEstado == 4)
            {
                return BadRequest(new { message = "No puedes inscribirte a esta actividad porque está pospuesta." });
            }

            //validaciones pa no volver a inscribirse 
            var existeInscripcion = await _context.Inscripciones
                .AnyAsync(i => i.IdUsuario == usuario.IdUsuario && i.IdActividad == request.IdActividad);
            if (existeInscripcion)
            {
                return BadRequest(new { message = "Ya estás inscrito en esta actividad." });
            }

            if (actividad.LimiteCupos <= 0)
            {
                return BadRequest(new { message = "No hay cupos disponibles para esta actividad." });
            }

            // Crea la inscripción jujujuj
            var inscripcion = new Inscripcione
            {
                IdUsuario = usuario.IdUsuario,
                IdActividad = actividad.IdActividad,
                FechaInscripcion = DateTime.Now,
                IdEstado = 5 
            };

            _context.Inscripciones.Add(inscripcion);

            actividad.LimiteCupos--;
            _context.Entry(actividad).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Inscripción registrada con éxito.",
                data = new
                {
                    inscripcion.IdInscripcion,
                    inscripcion.IdUsuario,
                    inscripcion.IdActividad,
                    inscripcion.FechaInscripcion,
                    inscripcion.IdEstado
                }
            });
        }
    }
}
