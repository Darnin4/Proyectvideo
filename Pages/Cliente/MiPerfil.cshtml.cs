using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VideojuegosStore.Models;
using VideojuegosStore.Data;
using VideojuegosStore.Filters;

namespace VideojuegosStore.Pages.Cliente
{
    [ClientRequired]
    public class MiPerfilModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public MiPerfilModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Usuario Usuario { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var userIdStr = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
            {
                return RedirectToPage("/Login");
            }

            Usuario = await _context.Usuarios.FindAsync(userId);

            if (Usuario == null)
            {
                return RedirectToPage("/Login");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var userIdStr = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
            {
                return RedirectToPage("/Login");
            }

            var usuarioDb = await _context.Usuarios.FindAsync(userId);
            if (usuarioDb == null)
            {
                return RedirectToPage("/Login");
            }

            // Actualiza los campos que el usuario puede modificar
            usuarioDb.Nombre = Usuario.Nombre;
            usuarioDb.Correo = Usuario.Correo;
            usuarioDb.Contraseña = Usuario.Contraseña;

            _context.Usuarios.Update(usuarioDb);
            await _context.SaveChangesAsync();

            TempData["Mensaje"] = "Perfil actualizado correctamente.";
            return RedirectToPage("/Cliente/Index");
        }
    }
}
