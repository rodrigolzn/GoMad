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
    public string BeneficiaryID { get; set; } = "GM-A1B2C3D4";

    public bool Submitted { get; set; } = false;

    public void OnGet() { }

    public IActionResult OnPost()
    {
        if (string.IsNullOrWhiteSpace(ContactPhone) || string.IsNullOrWhiteSpace(BeneficiaryID))
        {
            ModelState.AddModelError(string.Empty, "Debes rellenar ambos campos.");
            return Page();
        }

        // Guardar la solicitud en una cookie del beneficiario (demo)
        // En producción esto iría a base de datos.
        // Usamos una cookie "GoMad_EmergencyRequests" que acumula teléfonos separados por comas.
        var existingRequests = Request.Cookies["GoMad_EmergencyRequests"] ?? string.Empty;
        var list = new System.Collections.Generic.List<string>(
            existingRequests.Split(',', StringSplitOptions.RemoveEmptyEntries));

        if (!list.Contains(ContactPhone))
            list.Add(ContactPhone);

        var opts = new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(10), IsEssential = true };
        Response.Cookies.Append("GoMad_EmergencyRequests", string.Join(",", list), opts);

        Submitted = true;
        return Page();
    }
}
