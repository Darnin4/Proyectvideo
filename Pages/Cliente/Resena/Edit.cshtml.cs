using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VideojuegosStore.Data;
using VideojuegosStore.Filters;
using VideojuegosStore.Models;

namespace VideojuegosStore.Pages.Cliente.Resena
{
    [ClientRequired]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Rese�aUpdateDto Rese�a { get; set; }

        public Rese�a Rese�aOriginal { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr)) return RedirectToPage("/Login");

            Rese�aOriginal = await _context.Rese�as
                .Include(r => r.Videojuego)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (Rese�aOriginal == null) return NotFound();

            // Verificar que el usuario es el autor
            if (Rese�aOriginal.UsuarioId != int.Parse(userIdStr))
            {
                return Forbid();
            }

            Rese�a = new Rese�aUpdateDto
            {
                Calificacion = Rese�aOriginal.Calificacion,
                Comentario = Rese�aOriginal.Comentario
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (!ModelState.IsValid)
            {
                Rese�aOriginal = await _context.Rese�as
                    .Include(r => r.Videojuego)
                    .FirstOrDefaultAsync(r => r.Id == id);
                return Page();
            }

            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr)) return RedirectToPage("/Login");

            var rese�a = await _context.Rese�as.FindAsync(id);
            if (rese�a == null) return NotFound();

            // Verificar que el usuario es el autor
            if (rese�a.UsuarioId != int.Parse(userIdStr))
            {
                return Forbid();
            }

            rese�a.Calificacion = Rese�a.Calificacion;
            rese�a.Comentario = Rese�a.Comentario;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Rese�a actualizada exitosamente!";
            return RedirectToPage("./Index");
        }
    }

    public class Rese�aUpdateDto
    {
        [Range(1, 5, ErrorMessage = "La calificaci�n debe ser entre 1 y 5")]
        public int Calificacion { get; set; }

        [Required(ErrorMessage = "El comentario es obligatorio")]
        [StringLength(500, MinimumLength = 10, ErrorMessage = "El comentario debe tener entre 10 y 500 caracteres")]
        public string Comentario { get; set; }
    }
}