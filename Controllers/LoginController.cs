using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using VideojuegosStore.Data;
using VideojuegosStore.Models;

public class LoginModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public LoginModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Usuario Usuario { get; set; }

    public IActionResult OnPost()
    {
        var usuarioDB = _context.Usuarios
            .FirstOrDefault(u => u.Correo == Usuario.Correo && u.Contraseña == Usuario.Contraseña);

        if (usuarioDB == null)
        {
            // Redirigir si las credenciales no son correctas
            return Page(); // O mostrar un mensaje de error
        }

        if (usuarioDB.Rol == "Administrador")
        {
            return RedirectToPage("/Administrador/Index"); // Redirigir al administrador
        }
        else if (usuarioDB.Rol == "Cliente")
        {
            return RedirectToPage("/Cliente/Index"); // Redirigir al cliente
        }

        return Page(); // Si no es ningún rol, mostrar error
    }
}
