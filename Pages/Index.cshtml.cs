using System;
using System.ComponentModel.DataAnnotations;
using GoMad.Data;
using GoMad.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;

namespace GoMad.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly AppDbContext _dbContext;

    public IndexModel(ILogger<IndexModel> logger, AppDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    [BindProperty]
    [Required(ErrorMessage = "Introduce tu teléfono")]
    [Phone]
    public string Phone { get; set; } = string.Empty;

    [BindProperty]
    public string PhonePrefix { get; set; } = "+34";

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
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        _logger.LogInformation("Onboarding submitted: Phone={Phone} Postal={Postal}", Phone, PostalCode);

        var normalizedPhone = ((PhonePrefix ?? "+34").Trim() + (Phone ?? string.Empty).Trim()).Trim();
        var normalizedPostalCode = (PostalCode ?? string.Empty).Trim();
        var normalizedStreet = (Street ?? string.Empty).Trim();

        var usuario = await _dbContext.Usuarios
            .FirstOrDefaultAsync(u => u.Telefono == normalizedPhone);

        if (usuario is null)
        {
            usuario = new Usuario
            {
                Telefono = normalizedPhone,
                CodigoPostal = normalizedPostalCode,
                Calle = normalizedStreet
            };

            _dbContext.Usuarios.Add(usuario);
        }
        else
        {
            usuario.CodigoPostal = normalizedPostalCode;
            usuario.Calle = normalizedStreet;
        }

        await _dbContext.SaveChangesAsync();

        // Guardar marca de onboarding completado y datos en cookies (simple persistencia cliente)
        var opts = new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(10), IsEssential = true };
        Response.Cookies.Append("OnboardCompleted", "1", opts);
        Response.Cookies.Append("GoMad_Phone", usuario.Telefono, opts);
        Response.Cookies.Append("GoMad_PostalCode", usuario.CodigoPostal, opts);
        Response.Cookies.Append("GoMad_Street", usuario.Calle, opts);
        Response.Cookies.Append("GoMad_UserID", usuario.IdUsuario.ToString(), opts);

        return RedirectToPage("Main");
    }
}
