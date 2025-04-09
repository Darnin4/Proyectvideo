using Microsoft.AspNetCore.Mvc;

namespace VideojuegosStore.Controllers
{
    public class AdministradorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
