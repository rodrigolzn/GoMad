using System.ComponentModel.DataAnnotations;
using GoMad.Data;
using GoMad.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace GoMad.Pages;

public class VolunteerLoginModel : PageModel
{
    private readonly ILogger<VolunteerLoginModel> _logger;
    private readonly AppDbContext _dbContext;

    public VolunteerLoginModel(ILogger<VolunteerLoginModel> logger, AppDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    [BindProperty]
    [Required(ErrorMessage = "Introduce tu correo electrónico")]
    [EmailAddress(ErrorMessage = "Correo electrónico no válido")]
    public string Email { get; set; } = string.Empty;

    [BindProperty]
    [Required(ErrorMessage = "Introduce tu contraseña")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    public string? ErrorMessage { get; set; }

    public IActionResult OnGet()
    {
        // Si ya está logueado como voluntario, ir al dashboard
        if (Request.Cookies.TryGetValue("VolunteerLoggedIn", out var val) && val == "1")
        {
            return RedirectToPage("VolunteerDashboard");
        }
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var normalizedEmail = (Email ?? string.Empty).Trim().ToLowerInvariant();

        var volunteer = await _dbContext.Voluntarios
            .FirstOrDefaultAsync(v => v.Correo == normalizedEmail);

        if (volunteer is null || !BCrypt.Net.BCrypt.Verify(Password, volunteer.ContrasenaHash))
        {
            ErrorMessage = "Correo o contraseña incorrectos.";
            return Page();
        }

        _logger.LogInformation("Volunteer logged in: {Email}", normalizedEmail);

        var opts = new CookieOptions
        {
            Expires = DateTimeOffset.UtcNow.AddHours(8),
            IsEssential = true,
            HttpOnly = true
        };
        Response.Cookies.Append("VolunteerLoggedIn", "1", opts);
        Response.Cookies.Append("VolunteerEmail", normalizedEmail, opts);
        Response.Cookies.Append("VolunteerId", volunteer.IdVoluntario.ToString(), opts);

        return RedirectToPage("VolunteerDashboard");
    }
}
