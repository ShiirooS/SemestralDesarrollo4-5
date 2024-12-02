using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Actividades_Semestral.Models;
using MySqlConnector;
using System.Configuration;
using System.Data;

namespace Actividades_Semestral.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActividController : ControllerBase
    {
        private readonly GestorActividadesContext _context;
        private readonly string _connectionString;

        public ActividController(GestorActividadesContext context, IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("conexion")!;
            _context = context;
        }


        // GET: api/Activid
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var gestorActividades = await _context.Actividades
                .Include(a => a.IdEstadoNavigation)
                .Include(a => a.IdSubcategoriaNavigation)
                .ToListAsync();

            return Ok(gestorActividades);
        }

        // GET: api/Activid/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDetails(int id)
        {
            var actividade = await _context.Actividades
                .Include(a => a.IdEstadoNavigation)
                .Include(a => a.IdSubcategoriaNavigation)
                .FirstOrDefaultAsync(m => m.IdActividad == id);

            if (actividade == null)
            {
                return NotFound();
            }

            return Ok(actividade);
        }
        // GET: api/Activid/subcategoria/
        [HttpGet("subcategoria/{idSubcategoria}")]
        public async Task<IActionResult> GetBySubcategoria(int idSubcategoria)
        {
            var actividades = await _context.Actividades
                .Where(a => a.IdSubcategoria == idSubcategoria)
                .Include(a => a.IdEstadoNavigation)
                .Include(a => a.IdSubcategoriaNavigation)
                .ToListAsync();

            if (actividades == null || !actividades.Any())
            {
                return NotFound();
            }

            return Ok(actividades);
        }
        [HttpGet("estados-actividades")]
        public async Task<IActionResult> ObtenerEstadosActividades()
        {
            var estados = new List<EstadoActividade>();

            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new MySqlCommand("CALL ObtenerEstadosActividades();", connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        estados.Add(new EstadoActividade
                        {
                            IdEstado = reader.GetInt32("id_estado"),
                            NombreEstado = reader.GetString("nombre_estado")
                        });
                    }
                }
            }

            return Ok(estados);
        }


        // POST: api/Activid
        [HttpPost("crear")]
        public async Task<IActionResult> Create([FromBody] Actividade actividade)
        {
            actividade.IdEstado = 1;
            if (ModelState.IsValid)
            {
                _context.Add(actividade);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetDetails), new { id = actividade.IdActividad }, actividade);
            }
            return BadRequest(ModelState);
            
        }

        [HttpGet("{id}/detalles-completos")]
        public async Task<IActionResult> ObtenerActividadConUsuarios(int id)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new MySqlCommand("CALL GetActividadDetallesCompletos(@idActividad);", connection))
                {
                    command.Parameters.AddWithValue("@idActividad", id);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        var actividadDetalles = new ActividadDetalles();
                        while (await reader.ReadAsync())
                        {
                            if (actividadDetalles.InfoActividad.Count == 0)
                            {
                                actividadDetalles.InfoActividad = new Dictionary<string, object>
                                {
                                    { "IdActividad", reader["id_actividad"] },
                                    { "Imagen", reader["imagen_url"] },
                                    { "Nombre", reader["nombre"] },
                                    { "Descripcion", reader["descripcion"] },
                                    { "Fecha", reader["fecha"] },
                                    { "Hora", reader["hora"] },
                                    { "CuposMaximos", reader["limite_cupos"] },
                                    { "Costo", reader["costo"] },
                                    { "Requisitos", reader["requisitos"] },
                                    { "CuposOcupados", reader["cupos_ocupados"] },
                                    { "Estado", reader["estado"] }
                                 };
                            }
                            if (!reader.IsDBNull(reader.GetOrdinal("nombre_usuario")))
                            {
                                actividadDetalles.Usuarios.Add(new
                                {
                                    Nombre = reader["nombre_usuario"],
                                    Correo = reader["correo_usuario"]
                                });
                            }
                        }
                        if (actividadDetalles.InfoActividad.Count == 0)
                        {
                            return NotFound("Actividad no encontrada.");
                        }
                        return Ok(actividadDetalles);
                    }
                }
            }
        }


        // PUT: api/Activid/5 
        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(int id, [FromBody] Actividade actividade)
        {
            if (id != actividade.IdActividad)
            {
                return BadRequest("Activity ID mismatch.");
            }

            var actividadExistente = await _context.Actividades
                .Include(a => a.Inscripciones)
                .FirstOrDefaultAsync(a => a.IdActividad == id);

            if (actividadExistente == null)
            {
                return NotFound("Actividad no encontrada.");
            }
            actividadExistente.Nombre = actividade.Nombre;
            actividadExistente.Descripcion = actividade.Descripcion;
            actividadExistente.Fecha = actividade.Fecha;
            actividadExistente.Hora = actividade.Hora;
            actividadExistente.LimiteCupos = actividade.LimiteCupos;
            actividadExistente.Costo = actividade.Costo;
            actividadExistente.Requisitos = actividade.Requisitos;
            actividadExistente.IdEstado = actividade.IdEstado;

            // Actualizar el estado de las inscripciones asociadas
            foreach (var inscripcion in actividadExistente.Inscripciones)
            {
                // Sincronizar el estado de la inscripción con el estado de la actividad
                inscripcion.IdEstado = actividade.IdEstado;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ActividadeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var actividad = await _context.Actividades.FindAsync(id);
            if (actividad == null)
            {
                return NotFound("No se encontró la actividad con el ID proporcionado.");
            }

            var inscripciones = _context.Inscripciones.Where(i => i.IdActividad == id);
            _context.Inscripciones.RemoveRange(inscripciones);

            _context.Actividades.Remove(actividad);

            await _context.SaveChangesAsync();

            return NoContent();
        }
        private bool ActividadeExists(int id)
        {
            return _context.Actividades.Any(e => e.IdActividad == id);
        }


        [HttpGet("categoria/{idCategoria?}")]
        public async Task<IActionResult> GetByCategoria(int? idCategoria)
        {
            IQueryable<Actividade> query = _context.Actividades
                .Include(a => a.IdSubcategoriaNavigation)
                .ThenInclude(s => s.IdCategoriaNavigation);
            if (idCategoria.HasValue)
            {
                query = query.Where(a => a.IdSubcategoriaNavigation != null &&
                                         a.IdSubcategoriaNavigation.IdCategoria == idCategoria);
            }
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
                return Ok(new List<object>()); 
            }

            return Ok(actividades);
        }

        [HttpGet("actividades-completas")]
        public async Task<IActionResult> GetActividadesCompletas()
        {
            var actividadesCompletas = new List<ActividadesAdmin>();

            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new MySqlCommand("CALL GetActividadesCompletas();", connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        actividadesCompletas.Add(new ActividadesAdmin
                        {
                            IdActividad= reader.GetInt32("id"),
                            NombreActividad = reader.GetString("nombre_actividad"),
                            Estado = reader.GetString("estado"),
                            CuposMaximos = reader.GetInt32("cupos_maximos"),
                            CuposOcupados = reader.GetInt32("cupos_ocupados"),
                            Fecha = reader.GetDateTime("fecha")
                        });
                    }
                }
            }

            return Ok(actividadesCompletas);
        }
    }
}

