using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VideojuegosStore.Data;
using VideojuegosStore.Models;

namespace VideojuegosStore.Pages.Administrador.Resena
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Reseña> Reseñas { get; set; }
        public SelectList Videojuegos { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? VideojuegoId { get; set; }

        public async Task OnGetAsync()
        {
            // Obtener la lista de videojuegos para el dropdown
            Videojuegos = new SelectList(await _context.Videojuegos
                .OrderBy(v => v.Titulo)
                .ToListAsync(), "Id", "Titulo");

            // Consulta base
            var query = _context.Reseñas
                .Include(r => r.Videojuego)
                .Include(r => r.Usuario)
                .OrderByDescending(r => r.Fecha)
                .AsQueryable();

            // Aplicar filtro si existe
            if (VideojuegoId.HasValue)
            {
                query = query.Where(r => r.VideojuegoId == VideojuegoId.Value);
            }

            Reseñas = await query.ToListAsync();
        }
    }
}