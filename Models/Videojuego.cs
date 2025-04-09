using System.ComponentModel.DataAnnotations;

namespace VideojuegosStore.Models
{
    public class Videojuego
    {
        public int Id { get; set; }

        [Required]
        public string Titulo { get; set; } 

        [Required]
        public string Genero { get; set; }

        public string Plataforma { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal Precio { get; set; }

        public string ImagenUrl { get; set; }
        public string VideoUrl { get; set; }
        public int Stock { get; set; }
    }
}
