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
    public class ComprasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ComprasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Compras/Administrador
        [HttpGet("Administrador")]
        [AdminRequired]
        public async Task<ActionResult<IEnumerable<Compra>>> GetComprasParaAdmin()
        {
            return await _context.Compras
                .Include(c => c.Usuario)
                .Include(c => c.Detalles)
                    .ThenInclude(d => d.Videojuego)
                .OrderByDescending(c => c.FechaCompra)
                .ToListAsync();
        }

        // GET: api/Compras/Administrador/Videojuego/5
        [HttpGet("Administrador/Videojuego/{videojuegoId}")]
        [AdminRequired]
        public async Task<ActionResult<IEnumerable<Compra>>> GetComprasPorVideojuegoParaAdmin(int videojuegoId)
        {
            return await _context.Compras
                .Include(c => c.Usuario)
                .Include(c => c.Detalles)
                    .ThenInclude(d => d.Videojuego)
                .Where(c => c.Detalles.Any(d => d.VideojuegoId == videojuegoId))
                .OrderByDescending(c => c.FechaCompra)
                .ToListAsync();
        }

        // GET: api/Compras/Usuario/5
        [HttpGet("Usuario/{usuarioId}")]
        [ClientRequired]
        public async Task<ActionResult<IEnumerable<Compra>>> GetComprasPorUsuario(int usuarioId)
        {
            var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            // Solo permitir ver las propias compras
            if (currentUserId != usuarioId)
            {
                return Forbid();
            }

            return await _context.Compras
                .Include(c => c.Detalles)
                    .ThenInclude(d => d.Videojuego)
                .Where(c => c.UsuarioId == usuarioId)
                .OrderByDescending(c => c.FechaCompra)
                .ToListAsync();
        }

        // GET: api/Compras/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Compra>> GetCompra(int id)
        {
            var compra = await _context.Compras
                .Include(c => c.Usuario)
                .Include(c => c.Detalles)
                    .ThenInclude(d => d.Videojuego)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (compra == null)
            {
                return NotFound();
            }

            var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            // Solo el usuario o administrador puede ver la compra
            if (compra.UsuarioId != currentUserId && userRole != "Administrador")
            {
                return Forbid();
            }

            return compra;
        }

        // POST: api/Compras
        [HttpPost]
        [ClientRequired]
        public async Task<ActionResult<Compra>> CrearCompra(CompraCreateDto compraDto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            // Validar stock y calcular total
            var detalles = new List<DetalleCompra>();
            decimal total = 0;

            foreach (var item in compraDto.Items)
            {
                var videojuego = await _context.Videojuegos.FindAsync(item.VideojuegoId);
                if (videojuego == null)
                {
                    return BadRequest($"Videojuego con ID {item.VideojuegoId} no encontrado");
                }

                if (videojuego.Stock < item.Cantidad)
                {
                    return BadRequest($"Stock insuficiente para el videojuego: {videojuego.Titulo}");
                }

                detalles.Add(new DetalleCompra
                {
                    VideojuegoId = item.VideojuegoId,
                    Cantidad = item.Cantidad
                });

                total += videojuego.Precio * item.Cantidad;
            }

            // Crear la compra
            var compra = new Compra
            {
                UsuarioId = userId,
                FechaCompra = DateTime.UtcNow,
                Total = total,
                Detalles = detalles
            };

            // Actualizar stock
            foreach (var detalle in compra.Detalles)
            {
                var videojuego = await _context.Videojuegos.FindAsync(detalle.VideojuegoId);
                videojuego.Stock -= detalle.Cantidad;
                _context.Entry(videojuego).State = EntityState.Modified;
            }

            _context.Compras.Add(compra);
            await _context.SaveChangesAsync();

            // Cargar datos relacionados para la respuesta
            await _context.Entry(compra)
                .Reference(c => c.Usuario)
                .LoadAsync();

            await _context.Entry(compra)
                .Collection(c => c.Detalles)
                .Query()
                .Include(d => d.Videojuego)
                .LoadAsync();

            return CreatedAtAction("GetCompra", new { id = compra.Id }, compra);
        }

        // PUT: api/Compras/5/Detalle/5
        [HttpPut("{compraId}/Detalle/{detalleId}")]
        [ClientRequired]
        public async Task<IActionResult> ActualizarDetalleCompra(int compraId, int detalleId, DetalleUpdateDto detalleUpdateDto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var compra = await _context.Compras
                .Include(c => c.Detalles)
                .FirstOrDefaultAsync(c => c.Id == compraId);

            if (compra == null)
            {
                return NotFound("Compra no encontrada");
            }

            // Solo el usuario puede modificar su compra
            if (compra.UsuarioId != userId)
            {
                return Forbid();
            }

            var detalle = compra.Detalles.FirstOrDefault(d => d.Id == detalleId);
            if (detalle == null)
            {
                return NotFound("Detalle de compra no encontrado");
            }

            var videojuego = await _context.Videojuegos.FindAsync(detalle.VideojuegoId);
            if (videojuego == null)
            {
                return NotFound("Videojuego no encontrado");
            }

            // Calcular diferencia de stock
            int diferencia = detalleUpdateDto.Cantidad - detalle.Cantidad;

            if (videojuego.Stock < diferencia)
            {
                return BadRequest("Stock insuficiente para actualizar la cantidad");
            }

            // Actualizar stock
            videojuego.Stock -= diferencia;
            _context.Entry(videojuego).State = EntityState.Modified;

            // Actualizar detalle
            detalle.Cantidad = detalleUpdateDto.Cantidad;
            _context.Entry(detalle).State = EntityState.Modified;

            // Recalcular total
            compra.Total = compra.Detalles.Sum(d => d.Cantidad * d.Videojuego.Precio);
            _context.Entry(compra).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CompraExists(compraId))
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

        private bool CompraExists(int id)
        {
            return _context.Compras.Any(e => e.Id == id);
        }
    }

    // DTOs para las operaciones
    public class CompraCreateDto
    {
        public List<CompraItemDto> Items { get; set; }
    }

    public class CompraItemDto
    {
        public int VideojuegoId { get; set; }
        [Range(1, int.MaxValue)]
        public int Cantidad { get; set; }
    }

    public class DetalleUpdateDto
    {
        [Range(1, int.MaxValue)]
        public int Cantidad { get; set; }
    }
}