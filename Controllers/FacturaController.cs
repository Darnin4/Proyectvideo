using Microsoft.AspNetCore.Mvc;
using VideojuegosStore.Data;
using VideojuegosStore.Helpers;
using Microsoft.EntityFrameworkCore;

namespace VideojuegosStore.Controllers
{
    public class FacturaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FacturaController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("FacturaQR/{id:int}")]
        public async Task<IActionResult> GenerarFactura(int id)
        {
            var videojuego = await _context.Videojuegos
                .FirstOrDefaultAsync(v => v.Id == id);

            if (videojuego == null)
                return NotFound();

            string datosQr = $"Compra:{videojuego.Titulo}|Precio:{videojuego.Precio}";
            string qrBase64 = QrCodeHelper.GenerateQrCodeBase64(datosQr);

            ViewBag.Usuario = User.Identity?.Name ?? "Invitado";
            ViewBag.Videojuego = videojuego;
            ViewBag.QRImage = qrBase64;

            return View("~/Pages/Cliente/FacturaQR.cshtml");
        }
    }
}