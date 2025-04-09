using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using VideojuegosStore.Data;
using VideojuegosStore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace VideojuegosStore.Pages
{
    public class LoginModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public LoginModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public LoginViewModel Input { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u =>
                u.Correo == Input.Correo &&
                u.Contraseña == Input.Contraseña);

            if (usuario == null)
            {
                ModelState.AddModelError(string.Empty, "Credenciales incorrectas");
                return Page();
            }

            if (usuario.Rol != Input.Rol)
            {
                ModelState.AddModelError(string.Empty, "No tienes permisos para este rol");
                return Page();
            }

            // Configuración robusta de sesión
            HttpContext.Session.SetString("UserId", usuario.Id.ToString());
            HttpContext.Session.SetString("UserRole", usuario.Rol);
            HttpContext.Session.SetString("UserEmail", usuario.Correo);

            // Forzar guardado inmediato
            await HttpContext.Session.CommitAsync();

            // Redirección garantizada
            return usuario.Rol switch
            {
                "Administrador" => RedirectToPage("/Administrador/Index"),
                "Cliente" => RedirectToPage("/Cliente/Index"),
                _ => RedirectToPage("/Index")
            };
        }
    }
}