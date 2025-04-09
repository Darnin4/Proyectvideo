using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using VideojuegosStore.Data;
using VideojuegosStore.Models;

namespace VideojuegosStore.Pages.Administrador
{
    public class EditarUsuarioModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditarUsuarioModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Usuario Usuario { get; set; }

        public void OnGet(int id)
        {
            Usuario = _context.Usuarios.Find(id);
        }
    }
}