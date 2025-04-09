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
        public ReseñaUpdateDto Reseña { get; set; }

        public Reseña ReseñaOriginal { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr)) return RedirectToPage("/Login");

            ReseñaOriginal = await _context.Reseñas
                .Include(r => r.Videojuego)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (ReseñaOriginal == null) return NotFound();

            // Verificar que el usuario es el autor
            if (ReseñaOriginal.UsuarioId != int.Parse(userIdStr))
            {
                return Forbid();
            }

            Reseña = new ReseñaUpdateDto
            {
                Calificacion = ReseñaOriginal.Calificacion,
                Comentario = ReseñaOriginal.Comentario
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (!ModelState.IsValid)
            {
                ReseñaOriginal = await _context.Reseñas
                    .Include(r => r.Videojuego)
                    .FirstOrDefaultAsync(r => r.Id == id);
                return Page();
            }

            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr)) return RedirectToPage("/Login");

            var reseña = await _context.Reseñas.FindAsync(id);
            if (reseña == null) return NotFound();

            // Verificar que el usuario es el autor
            if (reseña.UsuarioId != int.Parse(userIdStr))
            {
                return Forbid();
            }

            reseña.Calificacion = Reseña.Calificacion;
            reseña.Comentario = Reseña.Comentario;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Reseña actualizada exitosamente!";
            return RedirectToPage("./Index");
        }
    }

    public class ReseñaUpdateDto
    {
        [Range(1, 5, ErrorMessage = "La calificación debe ser entre 1 y 5")]
        public int Calificacion { get; set; }

        [Required(ErrorMessage = "El comentario es obligatorio")]
        [StringLength(500, MinimumLength = 10, ErrorMessage = "El comentario debe tener entre 10 y 500 caracteres")]
        public string Comentario { get; set; }
    }
}