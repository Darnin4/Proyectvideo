using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VideojuegosStore.Data;
using VideojuegosStore.Models;
using System.Collections.Generic;
using System.Linq;

namespace VideojuegosStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UsuarioController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Usuario
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuarios()
        {
            if (HttpContext.Session.GetString("UserRole") != "Administrador")
            {
                return Unauthorized();
            }
            return await _context.Usuarios.ToListAsync();
        }

        // GET: api/Usuario/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Usuario>> GetUsuario(int id)
        {
            var currentUserId = HttpContext.Session.GetString("UserId");
            var currentUserRole = HttpContext.Session.GetString("UserRole");

            if (currentUserRole == "Cliente" && id.ToString() != currentUserId)
            {
                return Unauthorized();
            }

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return NotFound();

            return usuario;
        }

        // POST: api/Usuario
        [HttpPost]
        public async Task<ActionResult<Usuario>> PostUsuario(Usuario usuario)
        {
            if (HttpContext.Session.GetString("UserRole") != "Administrador")
            {
                return Unauthorized();
            }

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetUsuario", new { id = usuario.Id }, usuario);
        }

        // PUT: api/Usuario/5 (Para administradores - actualización completa)
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsuario(int id, Usuario usuario)
        {
            if (HttpContext.Session.GetString("UserRole") != "Administrador")
            {
                return Unauthorized();
            }

            if (id != usuario.Id) return BadRequest();

            var usuarioExistente = await _context.Usuarios.FindAsync(id);
            if (usuarioExistente == null) return NotFound();

            // Actualizar campos básicos
            usuarioExistente.Nombre = usuario.Nombre;
            usuarioExistente.Correo = usuario.Correo;
            usuarioExistente.Rol = usuario.Rol;

            // Solo actualizar contraseña si se proporcionó un valor
            if (!string.IsNullOrEmpty(usuario.Contraseña))
            {
                usuarioExistente.Contraseña = usuario.Contraseña;
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // PATCH: api/Usuario/5 (Para clientes - actualización parcial)
        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchUsuario(int id, [FromBody] Dictionary<string, object> updates)
        {
            var currentUserId = HttpContext.Session.GetString("UserId");
            var currentUserRole = HttpContext.Session.GetString("UserRole");

            if (currentUserRole != "Cliente" || id.ToString() != currentUserId)
            {
                return Unauthorized();
            }

            var usuarioExistente = await _context.Usuarios.FindAsync(id);
            if (usuarioExistente == null) return NotFound();

            // Campos permitidos para clientes
            var allowedFields = new[] { "nombre", "correo", "contraseña" };

            foreach (var update in updates)
            {
                if (!allowedFields.Contains(update.Key.ToLower())) continue;

                var property = typeof(Usuario).GetProperty(update.Key,
                    System.Reflection.BindingFlags.IgnoreCase |
                    System.Reflection.BindingFlags.Public |
                    System.Reflection.BindingFlags.Instance);

                if (property != null)
                {
                    property.SetValue(usuarioExistente, Convert.ChangeType(update.Value, property.PropertyType));
                }
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/Usuario/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            if (HttpContext.Session.GetString("UserRole") != "Administrador")
            {
                return Unauthorized();
            }

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return NotFound();

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}