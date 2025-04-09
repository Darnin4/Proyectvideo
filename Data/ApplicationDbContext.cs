using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;
using VideojuegosStore.Models;

namespace VideojuegosStore.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Videojuego> Videojuegos { get; set; }
        public DbSet<Reseña> Reseñas { get; set; }
        public DbSet<Compra> Compras { get; set; }
        public DbSet<DetalleCompra> DetalleCompras { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Compra>()
                .HasMany(c => c.Detalles)
                .WithOne(d => d.Compra)
                .HasForeignKey(d => d.CompraId);

            modelBuilder.Entity<DetalleCompra>()
                .HasOne(d => d.Videojuego)
                .WithMany()
                .HasForeignKey(d => d.VideojuegoId);

            // Agregas estas líneas para precisión decimal
            modelBuilder.Entity<Videojuego>()
                .Property(v => v.Precio)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Compra>()
                .Property(c => c.Total)
                .HasPrecision(18, 2);
        }
    }
}
