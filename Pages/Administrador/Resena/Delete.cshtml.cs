using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VideojuegosStore.Data;
using VideojuegosStore.Models;
using VideojuegosStore.Filters;

namespace VideojuegosStore.Pages.Administrador.Resena
{
    [AdminRequired]
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
            Reseña = await _context.Reseñas
                .Include(r => r.Videojuego)
                .Include(r => r.Usuario)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (Reseña == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var reseña = await _context.Reseñas.FindAsync(id);

            if (reseña != null)
            {
                _context.Reseñas.Remove(reseña);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Reseña eliminada correctamente.";
            }

            return RedirectToPage("./Index");
        }
    }
}