using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VideojuegosStore.Data;
using VideojuegosStore.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VideojuegosStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Requiere autenticación para todos los endpoints
    public class VideojuegosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public VideojuegosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Videojuegos
        [HttpGet]
        [AllowAnonymous] // Permite acceso sin autenticación
        public async Task<ActionResult<IEnumerable<Videojuego>>> GetVideojuegos()
        {
            return await _context.Videojuegos.ToListAsync();
        }

        // GET: api/Videojuegos/5
        [HttpGet("{id}")]
        [AllowAnonymous] // Permite acceso sin autenticación
        public async Task<ActionResult<Videojuego>> GetVideojuego(int id)
        {
            var videojuego = await _context.Videojuegos.FindAsync(id);

            if (videojuego == null)
            {
                return NotFound(new { Message = "Videojuego no encontrado" });
            }

            return Ok(videojuego);
        }

        // POST: api/Videojuegos
        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult<Videojuego>> PostVideojuego(Videojuego videojuego)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Videojuegos.Add(videojuego);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetVideojuego), new { id = videojuego.Id }, videojuego);
        }

        // PUT: api/Videojuegos/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> PutVideojuego(int id, Videojuego videojuego)
        {
            if (id != videojuego.Id)
            {
                return BadRequest(new { Message = "IDs no coinciden" });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Entry(videojuego).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VideojuegoExists(id))
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

        // DELETE: api/Videojuegos/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteVideojuego(int id)
        {
            var videojuego = await _context.Videojuegos.FindAsync(id);
            if (videojuego == null)
            {
                return NotFound(new { Message = "Videojuego no encontrado" });
            }

            _context.Videojuegos.Remove(videojuego);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool VideojuegoExists(int id)
        {
            return _context.Videojuegos.Any(e => e.Id == id);
        }
    }
}