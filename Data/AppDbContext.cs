using GoMad.Models;
using Microsoft.EntityFrameworkCore;

namespace GoMad.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Voluntario> Voluntarios => Set<Voluntario>();
    public DbSet<SolicitudEmergencia> SolicitudesEmergencia => Set<SolicitudEmergencia>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(x => x.IdUsuario);
            entity.Property(x => x.IdUsuario).ValueGeneratedOnAdd();
            entity.HasIndex(x => x.Telefono).IsUnique();
        });

        modelBuilder.Entity<Voluntario>(entity =>
        {
            entity.HasKey(x => x.IdVoluntario);
            entity.Property(x => x.IdVoluntario).ValueGeneratedOnAdd();
            entity.HasIndex(x => x.Correo).IsUnique();
        });

        modelBuilder.Entity<SolicitudEmergencia>(entity =>
        {
            entity.HasKey(x => x.IdSolicitudEmergencia);
            entity.Property(x => x.IdSolicitudEmergencia).ValueGeneratedOnAdd();
            entity.HasIndex(x => new { x.UsuarioId, x.TelefonoSolicitante }).IsUnique();

            entity.HasOne(x => x.Usuario)
                .WithMany()
                .HasForeignKey(x => x.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
