using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using VideojuegosStore.Models;
using VideojuegosStore.Data;
using VideojuegosStore.Filters;
using System.ComponentModel.DataAnnotations;

namespace VideojuegosStore.Pages.Cliente.Resena
{
    [ClientRequired]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Rese�aCreateDto Rese�a { get; set; }

        public Videojuego Videojuego { get; set; }

        public async Task<IActionResult> OnGetAsync(int videojuegoId)
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr)) return RedirectToPage("/Login"); // Par�ntesis a�adido

            Videojuego = await _context.Videojuegos.FindAsync(videojuegoId);
            if (Videojuego == null) return NotFound();

            Rese�a = new Rese�aCreateDto { VideojuegoId = videojuegoId };
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                Videojuego = await _context.Videojuegos.FindAsync(Rese�a.VideojuegoId);
                return Page();
            }

            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr)) return RedirectToPage("/Login"); // Par�ntesis a�adido

            var rese�a = new Rese�a
            {
                VideojuegoId = Rese�a.VideojuegoId,
                UsuarioId = int.Parse(userIdStr),
                Calificacion = Rese�a.Calificacion,
                Comentario = Rese�a.Comentario,
                Fecha = DateTime.Now
            };

            _context.Rese�as.Add(rese�a);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Rese�a creada exitosamente!";
            return RedirectToPage("./Index");
        }
    }

    public class Rese�aCreateDto
    {
        public int VideojuegoId { get; set; }

        [Range(1, 5, ErrorMessage = "La calificaci�n debe ser entre 1 y 5")]
        public int Calificacion { get; set; }

        [Required(ErrorMessage = "El comentario es obligatorio")]
        [StringLength(500, MinimumLength = 10, ErrorMessage = "El comentario debe tener entre 10 y 500 caracteres")]
        public string Comentario { get; set; }
    }
}