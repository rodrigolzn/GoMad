using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace GoMad.Pages;

public class MainModel : PageModel
{
    public string Phone { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string UserID { get; set; } = string.Empty;

    /// <summary>Solicitudes de contacto de emergencia pendientes (teléfono del solicitante).</summary>
    public List<string> EmergencyRequests { get; set; } = new();
    /// <summary>Contactos de emergencia ya aceptados.</summary>
    public List<string> AcceptedContacts { get; set; } = new();

    public void OnGet()
    {
        Phone = Request.Cookies["GoMad_Phone"] ?? string.Empty;
        PostalCode = Request.Cookies["GoMad_PostalCode"] ?? string.Empty;
        Street = Request.Cookies["GoMad_Street"] ?? string.Empty;
        UserID = Request.Cookies["GoMad_UserID"] ?? string.Empty;

        // Si no hay UserID, generar uno y guardarlo en cookie
        if (string.IsNullOrEmpty(UserID))
        {
            UserID = "GM-" + Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
            Response.Cookies.Append("GoMad_UserID", UserID, new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddYears(10),
                Path = "/",
                HttpOnly = false,
                IsEssential = true
            });
        }

        // Cargar solicitudes y contactos aceptados desde cookies (demo)
        var raw = Request.Cookies["GoMad_EmergencyRequests"] ?? string.Empty;
        if (!string.IsNullOrEmpty(raw))
            EmergencyRequests = new List<string>(raw.Split(',', StringSplitOptions.RemoveEmptyEntries));

        var accepted = Request.Cookies["GoMad_AcceptedContacts"] ?? string.Empty;
        if (!string.IsNullOrEmpty(accepted))
            AcceptedContacts = new List<string>(accepted.Split(',', StringSplitOptions.RemoveEmptyEntries));
    }
}
