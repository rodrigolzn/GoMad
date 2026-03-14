using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;

namespace GoMad.Pages;

public class EmergencyContactModel : PageModel
{
    [BindProperty]
    public string ContactPhone { get; set; } = string.Empty;

    [BindProperty]
    public string ContactPhonePrefix { get; set; } = "+34";

    [BindProperty]
    public string BeneficiaryID { get; set; } = string.Empty;

    public bool Submitted { get; set; } = false;

    public void OnGet() { }

    public IActionResult OnPost()
    {
        var fullPhone = ((ContactPhonePrefix ?? "+34").Trim() + (ContactPhone ?? string.Empty).Trim()).Trim();

        if (string.IsNullOrWhiteSpace(fullPhone) || string.IsNullOrWhiteSpace(BeneficiaryID))
        {
            ModelState.AddModelError(string.Empty, "Debes rellenar ambos campos.");
            return Page();
        }

        if (!int.TryParse(BeneficiaryID, out var beneficiaryId))
        {
            ModelState.AddModelError(string.Empty, "El ID del beneficiario debe ser numérico.");
            return Page();
        }

        // Guardar la solicitud en una cookie del beneficiario (demo)
        // En producción esto iría a base de datos.
        // Usamos una cookie "GoMad_EmergencyRequests" que acumula teléfonos separados por comas.
        var requestCookieName = $"GoMad_EmergencyRequests_{beneficiaryId}";
        var existingRequests = Request.Cookies[requestCookieName] ?? string.Empty;
        var list = new System.Collections.Generic.List<string>(
            existingRequests.Split(',', StringSplitOptions.RemoveEmptyEntries));

        if (!list.Contains(fullPhone))
            list.Add(fullPhone);

        var opts = new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(10), IsEssential = true };
        Response.Cookies.Append(requestCookieName, string.Join(",", list), opts);

        Submitted = true;
        return Page();
    }
}
