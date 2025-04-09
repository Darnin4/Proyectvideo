namespace VideojuegosStore.Models
{
    public class Compra
    {
        public int Id { get; set; }
        public DateTime FechaCompra { get; set; }

        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; }

        public decimal Total { get; set; }
        public List<DetalleCompra> Detalles { get; set; } = new();
    }
}
