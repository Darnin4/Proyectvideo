using Microsoft.AspNetCore.Mvc.RazorPages;
using VideojuegosStore.Models;
using VideojuegosStore.Data;
using VideojuegosStore.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace VideojuegosStore.Pages.Cliente.Compras
{
    [ClientRequired]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public CompraCreateViewModel Compra { get; set; }

        public Videojuego Videojuego { get; set; }

        public async Task<IActionResult> OnGetAsync(int videojuegoId)
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr)) return RedirectToPage("/Login");

            Videojuego = await _context.Videojuegos.FindAsync(videojuegoId);
            if (Videojuego == null) return NotFound();

            Compra = new CompraCreateViewModel
            {
                Items = new List<CompraItemViewModel>
                {
                    new CompraItemViewModel
                    {
                        VideojuegoId = videojuegoId,
                        Cantidad = 1,
                        PrecioUnitario = Videojuego.Precio
                    }
                }
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                if (Compra.Items.Any())
                {
                    Videojuego = await _context.Videojuegos.FindAsync(Compra.Items.First().VideojuegoId);
                }
                return Page();
            }

            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr)) return RedirectToPage("/Login");

            // Primero cargamos todos los videojuegos necesarios
            var videojuegoIds = Compra.Items.Select(i => i.VideojuegoId).ToList();
            var videojuegos = await _context.Videojuegos
                .Where(v => videojuegoIds.Contains(v.Id))
                .ToDictionaryAsync(v => v.Id);

            // Validar stock
            foreach (var item in Compra.Items)
            {
                if (!videojuegos.TryGetValue(item.VideojuegoId, out var videojuego))
                {
                    ModelState.AddModelError(string.Empty, $"Videojuego con ID {item.VideojuegoId} no encontrado");
                    return Page();
                }

                if (videojuego.Stock < item.Cantidad)
                {
                    ModelState.AddModelError(string.Empty, $"Stock insuficiente para el videojuego: {videojuego.Titulo}");
                    return Page();
                }
            }

            // Crear la compra
            var compra = new Compra
            {
                UsuarioId = int.Parse(userIdStr),
                FechaCompra = DateTime.Now,
                Detalles = Compra.Items.Select(i => new DetalleCompra
                {
                    VideojuegoId = i.VideojuegoId,
                    Cantidad = i.Cantidad,
                    Videojuego = videojuegos[i.VideojuegoId] 
                }).ToList()
            };

            // Calcular total usando los videojuegos ya cargados
            compra.Total = compra.Detalles.Sum(d => d.Cantidad * d.Videojuego.Precio);

            // Actualizar stock
            foreach (var detalle in compra.Detalles)
            {
                detalle.Videojuego.Stock -= detalle.Cantidad;
                _context.Entry(detalle.Videojuego).State = EntityState.Modified;
            }

            _context.Compras.Add(compra);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "¡Compra realizada con éxito!";
            return RedirectToPage("./Index");
        }
    }

    public class CompraCreateViewModel
    {
        public List<CompraItemViewModel> Items { get; set; }
    }

    public class CompraItemViewModel
    {
        public int VideojuegoId { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser al menos 1")]
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal => Cantidad * PrecioUnitario;
    }
}