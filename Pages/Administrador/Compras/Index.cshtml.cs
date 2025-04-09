using Microsoft.AspNetCore.Mvc.RazorPages;
using VideojuegosStore.Data;
using VideojuegosStore.Models;
using Microsoft.EntityFrameworkCore;
using VideojuegosStore.Filters;
using Microsoft.AspNetCore.Mvc;

namespace VideojuegosStore.Pages.Administrador.Compras
{
    [AdminRequired]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Compra> Compras { get; set; }
        public List<Videojuego> Videojuegos { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? VideojuegoId { get; set; }

        public async Task OnGetAsync()
        {
            Videojuegos = await _context.Videojuegos.ToListAsync();

            IQueryable<Compra> query = _context.Compras
                .Include(c => c.Usuario)
                .Include(c => c.Detalles)
                    .ThenInclude(d => d.Videojuego)
                .OrderByDescending(c => c.FechaCompra);

            if (VideojuegoId.HasValue)
            {
                query = query.Where(c => c.Detalles.Any(d => d.VideojuegoId == VideojuegoId.Value));
            }

            Compras = await query.ToListAsync();
        }
        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var compra = await _context.Compras
                .Include(c => c.Detalles)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (compra == null)
            {
                return NotFound();
            }

            // Restaurar el stock de los videojuegos
            foreach (var detalle in compra.Detalles)
            {
                var videojuego = await _context.Videojuegos.FindAsync(detalle.VideojuegoId);
                if (videojuego != null)
                {
                    videojuego.Stock += detalle.Cantidad;
                    _context.Entry(videojuego).State = EntityState.Modified;
                }
            }

            _context.Compras.Remove(compra);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Compra eliminada correctamente";
            return RedirectToPage();
        }
    }
}