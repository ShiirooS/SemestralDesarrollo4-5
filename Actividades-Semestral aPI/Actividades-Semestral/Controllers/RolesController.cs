using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Actividades_Semestral.Models;

namespace Actividades_Semestral.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly GestorActividadesContext _context;

        public RolesController(GestorActividadesContext context)
        {
            _context = context;
        }
      
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Role>>> GetRoles()
        {
            var roles = await _context.Roles.ToListAsync();

            if (roles == null || !roles.Any())
            {
                return NotFound("No se encontraron roles.");
            }

            return Ok(roles);
        }

        // pa validar el inicio de sesión que costó más que un choto
        [HttpPost("validar-login")]
        public async Task<ActionResult> ValidarLogin([FromBody] LoginRequest loginRequest)
        {
            if (loginRequest == null)
            {
                return BadRequest(new { message = "Datos de inicio de sesión inválidos." });
            }

            try
            {
                var usuario = await _context.Usuarios
                    .Include(u => u.IdRolNavigation)
                    .FirstOrDefaultAsync(u => u.Correo == loginRequest.Correo);

                if (usuario == null)
                {
                    return BadRequest(new { message = "Correo no registrado." });
                }

                if (usuario.contrasena != loginRequest.Contrasena)
                {
                    return BadRequest(new { message = "Contraseña incorrecta." });
                }

                if (usuario.IdRol != loginRequest.RolId)
                {
                    return BadRequest(new { message = "El correo no corresponde al perfil seleccionado." });
                }

                return Ok(new { message = "Acceso permitido.", idUsuario = usuario.IdUsuario, nombre = usuario.Nombre });
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, new { message = "Ocurrió un error interno en el servidor." });
            }
        }
    }
}
