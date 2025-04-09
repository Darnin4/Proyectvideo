using Microsoft.AspNetCore.Mvc;

namespace VideojuegosStore.Controllers
{
    public class ClienteController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
