using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VideojuegosStore.Data;
using VideojuegosStore.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using VideojuegosStore.Filters;
using System.ComponentModel.DataAnnotations;

namespace VideojuegosStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResenasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ResenasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Resenas/Videojuego/5
        [HttpGet("Videojuego/{videojuegoId}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Reseña>>> GetResenasPorVideojuego(int videojuegoId)
        {
            return await _context.Reseñas
                .Include(r => r.Usuario)
                .Include(r => r.Videojuego)
                .Where(r => r.VideojuegoId == videojuegoId)
                .OrderByDescending(r => r.Fecha)
                .ToListAsync();
        }

        // GET: api/Resenas/Usuario/5
        [HttpGet("Usuario/{usuarioId}")]
        [ClientRequired]
        public async Task<ActionResult<IEnumerable<Reseña>>> GetResenasPorUsuario(int usuarioId)
        {
            var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            // Solo permitir ver las propias reseñas
            if (currentUserId != usuarioId)
            {
                return Forbid();
            }

            return await _context.Reseñas
                .Include(r => r.Videojuego)
                .Where(r => r.UsuarioId == usuarioId)
                .OrderByDescending(r => r.Fecha)
                .ToListAsync();
        }

        // GET: api/Resenas/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<Reseña>> GetResena(int id)
        {
            var reseña = await _context.Reseñas
                .Include(r => r.Usuario)
                .Include(r => r.Videojuego)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reseña == null)
            {
                return NotFound();
            }

            return reseña;
        }

        // POST: api/Resenas
        [HttpPost]
        [ClientRequired]
        public async Task<ActionResult<Reseña>> CrearResena(ReseñaCreateDto resenaDto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var videojuego = await _context.Videojuegos.FindAsync(resenaDto.VideojuegoId);
            if (videojuego == null)
            {
                return NotFound("Videojuego no encontrado");
            }

            var usuario = await _context.Usuarios.FindAsync(userId);
            if (usuario == null)
            {
                return Unauthorized();
            }

            // Verificar si el usuario ya tiene una reseña para este videojuego
            var reseñaExistente = await _context.Reseñas
                .FirstOrDefaultAsync(r => r.UsuarioId == userId && r.VideojuegoId == resenaDto.VideojuegoId);

            if (reseñaExistente != null)
            {
                return BadRequest("Ya has creado una reseña para este videojuego");
            }

            var resena = new Reseña
            {
                UsuarioId = userId,
                VideojuegoId = resenaDto.VideojuegoId,
                Calificacion = resenaDto.Calificacion,
                Comentario = resenaDto.Comentario,
                Fecha = DateTime.UtcNow
            };

            _context.Reseñas.Add(resena);
            await _context.SaveChangesAsync();

            // Actualizar el promedio de calificaciones del videojuego
            await ActualizarPromedioCalificaciones(resenaDto.VideojuegoId);

            // Cargar datos relacionados para la respuesta
            resena.Usuario = usuario;
            resena.Videojuego = videojuego;

            return CreatedAtAction("GetResena", new { id = resena.Id }, resena);
        }

        // PUT: api/Resenas/5
        [HttpPut("{id}")]
        [ClientRequired]
        public async Task<IActionResult> ActualizarResena(int id, ReseñaUpdateDto resenaUpdateDto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var resena = await _context.Reseñas.FindAsync(id);
            if (resena == null)
            {
                return NotFound();
            }

            // Solo el autor puede editar la reseña
            if (resena.UsuarioId != userId)
            {
                return Forbid();
            }

            resena.Calificacion = resenaUpdateDto.Calificacion;
            resena.Comentario = resenaUpdateDto.Comentario;

            _context.Entry(resena).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();

                // Actualizar el promedio de calificaciones del videojuego
                await ActualizarPromedioCalificaciones(resena.VideojuegoId);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ResenaExists(id))
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

        // DELETE: api/Resenas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarResena(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            var resena = await _context.Reseñas.FindAsync(id);
            if (resena == null)
            {
                return NotFound();
            }

            // Solo el autor o un administrador pueden eliminar la reseña
            if (resena.UsuarioId != userId && userRole != "Administrador")
            {
                return Forbid();
            }

            var videojuegoId = resena.VideojuegoId;

            _context.Reseñas.Remove(resena);
            await _context.SaveChangesAsync();

            // Actualizar el promedio de calificaciones del videojuego
            await ActualizarPromedioCalificaciones(videojuegoId);

            return NoContent();
        }

        // Añade este nuevo endpoint al ResenasController
        [HttpGet("Administrador")]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult<IEnumerable<Reseña>>> GetResenasParaAdministrador()
        {
            return await _context.Reseñas
                .Include(r => r.Usuario)
                .Include(r => r.Videojuego)
                .OrderByDescending(r => r.Fecha)
                .ToListAsync();
        }

        // Endpoint con filtro por videojuego
        [HttpGet("Administrador/Videojuego/{videojuegoId}")]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult<IEnumerable<Reseña>>> GetResenasPorVideojuegoParaAdmin(int videojuegoId)
        {
            return await _context.Reseñas
                .Include(r => r.Usuario)
                .Include(r => r.Videojuego)
                .Where(r => r.VideojuegoId == videojuegoId)
                .OrderByDescending(r => r.Fecha)
                .ToListAsync();
        }

        private bool ResenaExists(int id)
        {
            return _context.Reseñas.Any(e => e.Id == id);
        }

        private async Task ActualizarPromedioCalificaciones(int videojuegoId)
        {
            // Este método ahora no hace nada, pero mantiene la compatibilidad
            // con las llamadas existentes en el controlador
            await Task.CompletedTask;

            // Si necesitas el promedio para algo, puedes calcularlo aquí
            // pero no lo guardarás en la base de datos
            // var promedio = await _context.Reseñas
            //     .Where(r => r.VideojuegoId == videojuegoId)
            //     .AverageAsync(r => (double?)r.Calificacion) ?? 0;
            // 
            // Puedes guardarlo en memoria o caché si lo necesitas
        }
    }

    // DTOs para las operaciones
    public class ReseñaCreateDto
    {
        public int VideojuegoId { get; set; }
        [Range(1, 5)]
        public int Calificacion { get; set; }
        [Required]
        [StringLength(500, MinimumLength = 10)]
        public string Comentario { get; set; }
    }

    public class ReseñaUpdateDto
    {
        [Range(1, 5)]
        public int Calificacion { get; set; }
        [Required]
        [StringLength(500, MinimumLength = 10)]
        public string Comentario { get; set; }
    }
}