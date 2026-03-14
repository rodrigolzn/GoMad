using GoMad.Models;
using Microsoft.EntityFrameworkCore;

namespace GoMad.Data;

public static class DatabaseInitializer
{
    public static async Task EnsureDatabaseAsync(AppDbContext dbContext)
    {
        await dbContext.Database.ExecuteSqlRawAsync(@"
CREATE TABLE IF NOT EXISTS usuarios (
    id_usuario INT NOT NULL AUTO_INCREMENT,
    telefono VARCHAR(20) NOT NULL,
    codigo_postal CHAR(5) NOT NULL,
    calle VARCHAR(50) NOT NULL,
    telefono_emergencias VARCHAR(20) NULL,
    PRIMARY KEY (id_usuario),
    UNIQUE KEY uq_usuarios_telefono (telefono)
);");

        await dbContext.Database.ExecuteSqlRawAsync(@"
CREATE TABLE IF NOT EXISTS voluntarios (
    id_voluntario INT UNSIGNED NOT NULL AUTO_INCREMENT,
    correo VARCHAR(70) NOT NULL,
    contrasena VARCHAR(255) NOT NULL,
    clave_zona CHAR(6) NOT NULL,
    PRIMARY KEY (id_voluntario),
    UNIQUE KEY uq_voluntarios_correo (correo)
);");

        // Sembrar los 5 voluntarios predefinidos
        var predefinedEmails = new[]
        {
            "voluntariogm1@gmail.com",
            "voluntariogm2@gmail.com",
            "voluntariogm3@gmail.com",
            "voluntariogm4@gmail.com",
            "voluntariogm5@gmail.com"
        };

        foreach (var email in predefinedEmails)
        {
            var exists = await dbContext.Voluntarios.AnyAsync(v => v.Correo == email);
            if (!exists)
            {
                dbContext.Voluntarios.Add(new Voluntario
                {
                    Correo = email,
                    ContrasenaHash = BCrypt.Net.BCrypt.HashPassword("1234asdfASDF"),
                    ClaveZona = "000000"
                });
            }
        }
        await dbContext.SaveChangesAsync();
    }
}
