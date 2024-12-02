using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Actividades_Semestral.Models;

namespace Actividades_Semestral.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriasController : ControllerBase
    {
        private readonly GestorActividadesContext _context;

        public CategoriasController(GestorActividadesContext context)
        {
            _context = context;
        }

        // todas las categorias 
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Categoria>>> GetCategorias()
        {
            var categorias = await _context.Categorias.ToListAsync();
            return Ok(categorias);
        }

        //categorias por nombre
        [HttpGet("nombre/{nombreCategoria}")]
        public async Task<ActionResult<Categoria>> GetCategoriaByNombre(string nombreCategoria)
        {
           
            var categoria = await _context.Categorias
                .FirstOrDefaultAsync(c => c.Nombre == nombreCategoria);

            
            if (categoria == null)
            {
                return NotFound($"La categoría con nombre '{nombreCategoria}' no se encontró.");
            }

            return Ok(categoria);
        }

        // pa obtener subcategorías de una categoría específica
        [HttpGet("{id}/subcategorias")]
        public async Task<ActionResult<IEnumerable<Subcategoria>>> GetSubcategoriasByCategoriaId(int id)
        {
            var subcategorias = await _context.Subcategorias
                .Where(s => s.IdCategoria == id)
                .ToListAsync();

            if (subcategorias == null || !subcategorias.Any())
            {
                return NotFound($"No se encontraron subcategorías para la categoría con ID {id}.");
            }

            return Ok(subcategorias);
        }
    }
}
