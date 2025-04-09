using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VideojuegosStore.Data;
using VideojuegosStore.Filters;
using VideojuegosStore.Models;

namespace VideojuegosStore.Pages.Cliente.Resena
{
    [ClientRequired]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Reseña> Reseñas { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr)) return RedirectToPage("/Login");

            var userId = int.Parse(userIdStr);

            Reseñas = await _context.Reseñas
                .Include(r => r.Videojuego)
                .Where(r => r.UsuarioId == userId)
                .OrderByDescending(r => r.Fecha)
                .ToListAsync();

            return Page();
        }
    }
}