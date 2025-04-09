using Microsoft.AspNetCore.Mvc.RazorPages;
using VideojuegosStore.Data;
using VideojuegosStore.Models;
using Microsoft.EntityFrameworkCore;
using VideojuegosStore.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VideojuegosStore.Pages.Cliente
{
    [ClientRequired]
    public class JuegosModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public JuegosModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public string Plataforma { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Genero { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Busqueda { get; set; }

        public List<Videojuego> Videojuegos { get; set; } // Cambiado de 'Videojuego' a 'Videojuegos'

        public async Task OnGetAsync()
        {
            IQueryable<Videojuego> query = _context.Videojuegos
                .Where(v => v.Stock > 0); // Solo mostrar disponibles

            if (!string.IsNullOrEmpty(Plataforma))
            {
                query = query.Where(v => v.Plataforma == Plataforma);
            }

            if (!string.IsNullOrEmpty(Genero))
            {
                query = query.Where(v => v.Genero == Genero);
            }

            if (!string.IsNullOrEmpty(Busqueda))
            {
                query = query.Where(v => v.Titulo.Contains(Busqueda));
            }

            Videojuegos = await query.ToListAsync(); // Cambiado para asignar a la propiedad correcta
        }
    }
}