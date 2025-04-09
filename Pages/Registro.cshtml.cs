using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VideojuegosStore.Data;
using VideojuegosStore.Models;

namespace VideojuegosStore.Pages
{
    public class RegistroModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public RegistroModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Usuario Usuario { get; set; }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Validar si el correo ya existe
            if (await _context.Usuarios.AnyAsync(u => u.Correo == Usuario.Correo))
            {
                ModelState.AddModelError("Usuario.Correo", "Ya existe un usuario con este correo electrónico");
                return Page();
            }

            // Asignar rol Cliente por defecto
            Usuario.Rol = "Cliente";

            _context.Usuarios.Add(Usuario);
            await _context.SaveChangesAsync();

            // Redirigir al Login después del registro
            return RedirectToPage("/Login");
        }
    }
}