using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using VideojuegosStore.Filters;

namespace VideojuegosStore.Pages.Administrador
{
    [AdminRequired]
    public class UsuariosModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
