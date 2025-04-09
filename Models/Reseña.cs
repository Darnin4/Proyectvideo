namespace VideojuegosStore.Models
{
    public class Reseña
    {
        public int Id { get; set; }

        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; }

        public int VideojuegoId { get; set; }
        public Videojuego Videojuego { get; set; }

        public int Calificacion { get; set; } // de 1 a 5
        public string Comentario { get; set; }
        public DateTime Fecha { get; set; }
    }
}
