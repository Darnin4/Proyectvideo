using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VideojuegosStore.Data;
using VideojuegosStore.Filters;
using VideojuegosStore.Models;

namespace VideojuegosStore.Pages.Cliente.Resena
{
    [ClientRequired]
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DeleteModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Reseña Reseña { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr)) return RedirectToPage("/Login");

            Reseña = await _context.Reseñas
                .Include(r => r.Videojuego)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (Reseña == null) return NotFound();

            // Verificar que el usuario es el autor
            if (Reseña.UsuarioId != int.Parse(userIdStr))
            {
                return Forbid();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr)) return RedirectToPage("/Login");

            var reseña = await _context.Reseñas.FindAsync(id);
            if (reseña == null) return NotFound();

            // Verificar que el usuario es el autor
            if (reseña.UsuarioId != int.Parse(userIdStr))
            {
                return Forbid();
            }

            _context.Reseñas.Remove(reseña);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Reseña eliminada exitosamente!";
            return RedirectToPage("./Index");
        }
    }
}