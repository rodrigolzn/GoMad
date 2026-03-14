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
    }
}
