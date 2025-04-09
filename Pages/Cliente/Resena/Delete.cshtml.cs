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
        public Rese�a Rese�a { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr)) return RedirectToPage("/Login");

            Rese�a = await _context.Rese�as
                .Include(r => r.Videojuego)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (Rese�a == null) return NotFound();

            // Verificar que el usuario es el autor
            if (Rese�a.UsuarioId != int.Parse(userIdStr))
            {
                return Forbid();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr)) return RedirectToPage("/Login");

            var rese�a = await _context.Rese�as.FindAsync(id);
            if (rese�a == null) return NotFound();

            // Verificar que el usuario es el autor
            if (rese�a.UsuarioId != int.Parse(userIdStr))
            {
                return Forbid();
            }

            _context.Rese�as.Remove(rese�a);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Rese�a eliminada exitosamente!";
            return RedirectToPage("./Index");
        }
    }
}