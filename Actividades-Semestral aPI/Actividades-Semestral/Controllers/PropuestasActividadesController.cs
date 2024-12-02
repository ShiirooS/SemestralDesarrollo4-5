using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Actividades_Semestral.Models;
using MySqlConnector;

namespace Actividades_Semestral.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PropuestasActividadesController : ControllerBase
    {
        private readonly GestorActividadesContext _context;

        public PropuestasActividadesController(GestorActividadesContext context)
        {
            _context = context;
        }

        // GET: api/PropuestasActividades
        [HttpGet]
        public async Task<IActionResult> ObtenerTodo()
        {
            var propuestasActividades = await _context.PropuestasActividades
                .Include(p => p.IdEstadoNavigation)
                .Include(p => p.IdSubcategoriaNavigation)
                .Include(p => p.IdUsuarioNavigation)
                .ToListAsync();

            return Ok(propuestasActividades);
        }

        // GET: api/PropuestasActividades/5
        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerDetalles(int id)
        {
            var propuestasActividade = await _context.PropuestasActividades
                .Include(p => p.IdEstadoNavigation)
                .Include(p => p.IdSubcategoriaNavigation)
                .Include(p => p.IdUsuarioNavigation)
                .FirstOrDefaultAsync(m => m.IdPropuesta == id);

            if (propuestasActividade == null)
            {
                return NotFound();
            }

            return Ok(propuestasActividade);
        }
        // --------------------------------------OBETENR TODAS LAS PROPUESTAS
        [HttpGet("propuestas-completas")]
        public async Task<IActionResult> GetPropuestasCompletas()
        {
            var propuestasCompletas = new List<object>();

            using (var connection = new MySqlConnection(_context.Database.GetConnectionString()))
            {
                await connection.OpenAsync();

                using (var command = new MySqlCommand("CALL GetPropuestasCompletas();", connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        propuestasCompletas.Add(new
                        {
                            IdPropuesta = reader.GetInt32("IdPropuesta"),
                            NombrePropuesta = reader.GetString("NombrePropuesta"),
                            Estado = reader.GetString("Estado"),
                            NombreUsuario = reader.GetString("NombreUsuario")
                        });
                    }
                }
            }

            return Ok(propuestasCompletas);
        }



        //--------------OBTENER DETALLES DE PROPUESTAS--------------------------------------------------------

        [HttpGet("{id}/detalles-completos")]
        public async Task<IActionResult> ObtenerPropuestaDetallesCompletos(int id)
        {
            using (var connection = new MySqlConnection(_context.Database.GetConnectionString()))
            {
                await connection.OpenAsync();

                using (var command = new MySqlCommand("CALL ObtenerPropuestaDetallesCompletos(@idPropuesta);", connection))
                {
                    command.Parameters.AddWithValue("@idPropuesta", id);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (!await reader.ReadAsync())
                        {
                            return NotFound("Propuesta no encontrada.");
                        }

                        var propuestaDetalles = new
                        {
                            IdPropuesta = reader["id_propuesta"],
                            Imagen = reader["imagen"],
                            Nombre = reader["nombre"],
                            Descripcion = reader["descripcion"],
                            Fecha = reader["fecha"],
                            lugar = reader["lugar"],
                            Hora = reader["hora"],
                            LimiteCupos = reader["limite_cupos"],
                            Costo = reader["costo"],
                            Requisitos = reader["requisitos"],
                            Estado = reader["estado"],
                            NombreUsuario = reader["nombre_usuario"],
                            CorreoUsuario = reader["correo_usuario"]
                        };

                        return Ok(propuestaDetalles);
                    }
                }
            }
        }
        //-----------------------aprobar------------
        [HttpPost("{id}/aprobar")]
        public async Task<IActionResult> AprobarPropuesta(int id)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var propuesta = await _context.PropuestasActividades
                        .Include(p => p.IdSubcategoriaNavigation)
                        .FirstOrDefaultAsync(p => p.IdPropuesta == id);

                    if (propuesta == null)
                        return NotFound(new { message = "Propuesta no encontrada." });

                    if (propuesta.IdEstado == 6) // Estado "Aprobada"
                        return BadRequest(new { message = "La propuesta ya ha sido aprobada." });
                    if (propuesta.IdEstado == 7)
                        return BadRequest(new { message = "La Propuesta no puede ser rechazada" });
                    var nuevaActividad = new Actividade
                    {
                        Nombre = propuesta.Nombre,
                        Descripcion = propuesta.Descripcion,
                        Fecha = propuesta.Fecha,
                        Hora = propuesta.Hora,
                        LimiteCupos = propuesta.LimiteCupos,
                        Costo = propuesta.Costo,
                        Requisitos = propuesta.Requisitos,
                        ImagenUrl = propuesta.ImagenUrl,
                        IdSubcategoria = propuesta.IdSubcategoria,
                        IdEstado = 1, // Estado "Activo",
                        Lugar = propuesta.Lugar

                    };

                    _context.Actividades.Add(nuevaActividad);

                    propuesta.IdEstado = 6; // Cambiar estado a "Aprobada"

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return Ok(nuevaActividad);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(500, new { message = "Error interno del servidor.", detail = ex.Message });
                }
            }
        }
        //--------------------- RECHAZAR-------------------------------------
        [HttpPost("{id}/rechazar")]
        public async Task<IActionResult> RechazarPropuesta(int id, [FromBody] string comentarioRechazo)
        {
            try
            {
                var propuesta = await _context.PropuestasActividades.FirstOrDefaultAsync(p => p.IdPropuesta == id);

                if (propuesta == null)
                    return NotFound(new { message = "Propuesta no encontrada." });

                if (propuesta.IdEstado == 6)
                    return BadRequest(new { message = "La propuesta no puede ser rechazada." });

                if (propuesta.IdEstado == 7)
                    return BadRequest(new { message = "La propuesta ya ha sido rechazada." });
                // Actualizar estado y comentario
                propuesta.IdEstado = 7; 
                propuesta.ComentarioRechazo = comentarioRechazo;

                await _context.SaveChangesAsync();

                return Ok(new { message = "Propuesta rechazada exitosamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error al rechazar la propuesta.",
                    detail = ex.Message
                });
            }
        }
        //---------------------------CREAR PROPUESTA----------------------------------------

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] PropuestasActividade propuesta)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        message = "Los datos enviados son inválidos.",
                        errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                    });
                }

                // Verificar unicidad del nombre y del lugar de la propuesta
                if (_context.PropuestasActividades.Any(p => p.Nombre == propuesta.Nombre))
                {
                    return Conflict(new { field = "Nombre", message = "Ya existe una propuesta con este nombre." });
                }
                
                // Validar si ya existe una propuesta similar con nombre, fecha, hora, lugar
                var propuestaExistente = await _context.PropuestasActividades
                    .FirstOrDefaultAsync(p =>
                        p.Nombre == propuesta.Nombre &&
                        p.Lugar == propuesta.Lugar &&
                        p.Fecha == propuesta.Fecha &&
                        p.Hora == propuesta.Hora);

                if (propuestaExistente != null)
                {
                    return Conflict(new { field = "Nombre, Lugar, Fecha, Hora", message = "Ya existe una propuesta con esta combinación de nombre, lugar, fecha y hora." });
                }



                // Verificar la unicidad de los atributos con un diccionario, 

                // Diccionario de validaciones con mensajes específicos
                /* var validaciones = new List<(string Campo, string Mensaje, Func<PropuestasActividade, bool> Condicion)>
                 {
                ("Nombre", "Ya existe una propuesta con este nombre.", p => p.Nombre == propuesta.Nombre),
                   ("Lugar", "Ya existe una propuesta en este lugar.", p => p.Lugar == propuesta.Lugar),
                   ("Fecha", "Ya existe una propuesta en esta fecha.", p => p.Fecha == propuesta.Fecha),
                  ("Hora", "Ya existe una propuesta en esta hora.", p => p.Hora == propuesta.Hora)
                    };
                 foreach (var (campo, mensaje, condicion) in validaciones)
                 {
                     if (_context.PropuestasActividades.Any(condicion))
                     {
                         return Conflict(new
                         {
                             message = mensaje,
                             campo = campo
                         });
                     }
                 }*/


                if (!_context.Subcategorias.Any(s => s.IdSubcategoria == propuesta.IdSubcategoria))
                {
                    return BadRequest(new { message = "La subcategoría no existe." });
                }

                // Validar que el usuario existe
                if (!_context.Usuarios.Any(u => u.IdUsuario == propuesta.IdUsuario))
                {
                    return BadRequest(new { message = "El usuario no existe." });
                }

                propuesta.IdEstado = 5;

                _context.PropuestasActividades.Add(propuesta);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(ObtenerDetalles), new { id = propuesta.IdPropuesta }, propuesta);
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new
                {
                    message = "Error al guardar en la base de datos.",
                    detail = ex.InnerException?.Message ?? ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error interno del servidor.",
                    detail = ex.Message
                });
            }
        }

        private bool PropuestasActividadeExists(int id)
        {
            return _context.PropuestasActividades.Any(e => e.IdPropuesta == id);
        }
    }
}
