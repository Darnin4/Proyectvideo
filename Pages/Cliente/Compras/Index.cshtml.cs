using Microsoft.AspNetCore.Mvc.RazorPages;
using VideojuegosStore.Data;
using VideojuegosStore.Models;
using Microsoft.EntityFrameworkCore;
using VideojuegosStore.Filters;

namespace VideojuegosStore.Pages.Cliente.Compras
{
    [ClientRequired]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Compra> Compras { get; set; }

        public async Task OnGetAsync()
        {
            var userId = int.Parse(HttpContext.Session.GetString("UserId"));

            Compras = await _context.Compras
                .Include(c => c.Detalles)
                    .ThenInclude(d => d.Videojuego)
                .Where(c => c.UsuarioId == userId)
                .OrderByDescending(c => c.FechaCompra)
                .ToListAsync();
        }
    }
}