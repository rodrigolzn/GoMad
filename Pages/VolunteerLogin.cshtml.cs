using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GoMad.Pages;

public class VolunteerLoginModel : PageModel
{
    private const string ValidAccessKey = "230806";

    private readonly ILogger<VolunteerLoginModel> _logger;

    public VolunteerLoginModel(ILogger<VolunteerLoginModel> logger)
    {
        _logger = logger;
    }

    [BindProperty]
    [Required(ErrorMessage = "Introduce tu correo electrónico")]
    [EmailAddress(ErrorMessage = "Correo electrónico no válido")]
    public string Email { get; set; } = string.Empty;

    [BindProperty]
    [Required(ErrorMessage = "Introduce tu contraseña")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [BindProperty]
    [Required(ErrorMessage = "Introduce la clave de acceso del ayuntamiento")]
    public string AccessKey { get; set; } = string.Empty;

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

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        // Validar clave de acceso del ayuntamiento
        if (AccessKey != ValidAccessKey)
        {
            ErrorMessage = "Clave de acceso incorrecta. Solicita la clave válida a tu ayuntamiento.";
            return Page();
        }

        // Validar credenciales (en producción usar Identity/OAuth)
        if (!string.IsNullOrWhiteSpace(Email) && !string.IsNullOrWhiteSpace(Password))
        {
            _logger.LogInformation("Volunteer logged in: {Email}", Email);

            var opts = new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddHours(8),
                IsEssential = true,
                HttpOnly = true
            };
            Response.Cookies.Append("VolunteerLoggedIn", "1", opts);
            Response.Cookies.Append("VolunteerEmail", Email, opts);

            return RedirectToPage("VolunteerDashboard");
        }

        ErrorMessage = "Credenciales incorrectas. Comprueba tu email y contraseña.";
        return Page();
    }
}
