using System.ComponentModel.DataAnnotations;

namespace VideojuegosStore.Models
{
    public class Usuario
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "Formato de correo inválido")]
        public string Correo { get; set; } = string.Empty;
        public string Contraseña { get; set; } = string.Empty;

        public string Rol { get; set; } = "Cliente"; // Valor por defecto

    }
}
