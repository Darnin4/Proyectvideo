using System.ComponentModel.DataAnnotations.Schema;

namespace VideojuegosStore.Models
{
    public class DetalleCompra
    {
        public int Id { get; set; }
        public int CompraId { get; set; }
        public Compra Compra { get; set; }
        public int VideojuegoId { get; set; }
        public Videojuego Videojuego { get; set; }
        public int Cantidad { get; set; }

        [NotMapped]
        public decimal PrecioUnitario => Videojuego?.Precio ?? 0;
    }
}
