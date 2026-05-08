using GoMad.Models;
using Microsoft.EntityFrameworkCore;

namespace GoMad.Data;

public static class DatabaseInitializer
{
    public static async Task EnsureDatabaseAsync(AppDbContext dbContext)
    {
        await dbContext.Database.EnsureCreatedAsync();

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
