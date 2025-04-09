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
        public Rese�a Rese�a { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Rese�a = await _context.Rese�as
                .Include(r => r.Videojuego)
                .Include(r => r.Usuario)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (Rese�a == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var rese�a = await _context.Rese�as.FindAsync(id);

            if (rese�a != null)
            {
                _context.Rese�as.Remove(rese�a);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Rese�a eliminada correctamente.";
            }

            return RedirectToPage("./Index");
        }
    }
}