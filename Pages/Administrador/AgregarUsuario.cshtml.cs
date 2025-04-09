using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using VideojuegosStore.Data;
using VideojuegosStore.Models;
using VideojuegosStore.Filters;
using Microsoft.EntityFrameworkCore;

namespace VideojuegosStore.Pages.Administrador
{
    [AdminRequired]
    public class AgregarUsuarioModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public AgregarUsuarioModel(ApplicationDbContext context)
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

            // Validación adicional del correo
            var correoExiste = await _context.Usuarios.AnyAsync(u => u.Correo == Usuario.Correo);
            if (correoExiste)
            {
                ModelState.AddModelError("Usuario.Correo", "Ya existe un usuario con este correo electrónico");
                return Page();
            }

            _context.Usuarios.Add(Usuario);
            await _context.SaveChangesAsync();

            return RedirectToPage("Usuarios");
        }
    }
}