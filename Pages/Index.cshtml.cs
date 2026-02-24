using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;

namespace GoMad.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    [BindProperty]
    [Required(ErrorMessage = "Introduce tu teléfono")]
    [Phone]
    public string Phone { get; set; } = string.Empty;

    [BindProperty]
    [Required(ErrorMessage = "Introduce tu código postal")]
    [RegularExpression(@"^[0-9]{4,5}$", ErrorMessage = "Código postal inválido")]
    public string PostalCode { get; set; } = string.Empty;

    [BindProperty]
    [Required(ErrorMessage = "Introduce la calle")]
    public string Street { get; set; } = string.Empty;

    public IActionResult OnGet()
    {
        // Si ya completó el onboarding, redirigir a la página principal (Main)
        if (Request.Cookies.TryGetValue("OnboardCompleted", out var val) && val == "1")
        {
            return RedirectToPage("Main");
        }

        return Page();
    }

    public IActionResult OnGetRedirectIfDone()
    {
        // kept for compatibility
        return OnGet();
    }

    public override void OnPageHandlerExecuting(Microsoft.AspNetCore.Mvc.Filters.PageHandlerExecutingContext context)
    {
        base.OnPageHandlerExecuting(context);
    }

    // The OnGet method is already defined above, so we do not need to redefine it here.
    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        _logger.LogInformation("Onboarding submitted: Phone={Phone} Postal={Postal}", Phone, PostalCode);

        // Guardar marca de onboarding completado y datos en cookies (simple persistencia cliente)
        var opts = new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(10), IsEssential = true };
        Response.Cookies.Append("OnboardCompleted", "1", opts);
        Response.Cookies.Append("GoMad_Phone", Phone ?? string.Empty, opts);
        Response.Cookies.Append("GoMad_PostalCode", PostalCode ?? string.Empty, opts);
        Response.Cookies.Append("GoMad_Street", Street ?? string.Empty, opts);

        // Generar ID único para el beneficiario (si no existe ya)
        if (!Request.Cookies.ContainsKey("GoMad_UserID"))
        {
            var userId = "GM-" + Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
            Response.Cookies.Append("GoMad_UserID", userId, opts);
        }

        return RedirectToPage("Main");
    }
}
