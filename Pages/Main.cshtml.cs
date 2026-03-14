using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using GoMad.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GoMad.Pages;

public class MainModel : PageModel
{
    private readonly AppDbContext _dbContext;

    public MainModel(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public string Phone { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string UserID { get; set; } = string.Empty;
    /// <summary>ID formateado con ceros a la izquierda para mostrar en pantalla (10 dígitos).</summary>
    public string UserIDFormatted { get; set; } = string.Empty;

    /// <summary>Solicitudes de contacto de emergencia pendientes (teléfono del solicitante).</summary>
    public List<string> EmergencyRequests { get; set; } = new();
    /// <summary>Contactos de emergencia ya aceptados.</summary>
    public List<string> AcceptedContacts { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        Phone = Request.Cookies["GoMad_Phone"] ?? string.Empty;
        PostalCode = Request.Cookies["GoMad_PostalCode"] ?? string.Empty;
        Street = Request.Cookies["GoMad_Street"] ?? string.Empty;
        UserID = Request.Cookies["GoMad_UserID"] ?? string.Empty;

        if (int.TryParse(UserID, out var userId))
        {
            var usuario = await _dbContext.Usuarios.FirstOrDefaultAsync(u => u.IdUsuario == userId);
            if (usuario is not null)
            {
                Phone = usuario.Telefono;
                PostalCode = usuario.CodigoPostal;
                Street = usuario.Calle;
                UserID = usuario.IdUsuario.ToString();
                UserIDFormatted = usuario.IdUsuario.ToString("D10");

                if (!string.IsNullOrWhiteSpace(usuario.TelefonoEmergencias))
                {
                    AcceptedContacts = new List<string> { usuario.TelefonoEmergencias };
                }
            }
        }

        var requestsCookieName = $"GoMad_EmergencyRequests_{UserID}";
        var raw = Request.Cookies[requestsCookieName] ?? string.Empty;
        if (!string.IsNullOrEmpty(raw))
            EmergencyRequests = new List<string>(raw.Split(',', StringSplitOptions.RemoveEmptyEntries));

        var accepted = Request.Cookies["GoMad_AcceptedContacts"] ?? string.Empty;
        if (!string.IsNullOrEmpty(accepted))
            AcceptedContacts = AcceptedContacts
                .Concat(accepted.Split(',', StringSplitOptions.RemoveEmptyEntries))
                .Distinct()
                .ToList();

        return Page();
    }

    public async Task<IActionResult> OnPostResolveEmergencyAsync(string phone, bool accept)
    {
        var userIdRaw = Request.Cookies["GoMad_UserID"] ?? string.Empty;
        if (!int.TryParse(userIdRaw, out var userId))
        {
            return RedirectToPage();
        }

        var requestsCookieName = $"GoMad_EmergencyRequests_{userId}";
        var pending = ParseCookieList(Request.Cookies[requestsCookieName]);
        pending.RemoveAll(x => x == phone);
        Response.Cookies.Append(requestsCookieName, string.Join(",", pending), BuildLongCookie());

        if (accept)
        {
            var accepted = ParseCookieList(Request.Cookies["GoMad_AcceptedContacts"]);
            if (!accepted.Contains(phone))
            {
                accepted.Add(phone);
            }

            Response.Cookies.Append("GoMad_AcceptedContacts", string.Join(",", accepted), BuildLongCookie());

            var usuario = await _dbContext.Usuarios.FirstOrDefaultAsync(u => u.IdUsuario == userId);
            if (usuario is not null)
            {
                usuario.TelefonoEmergencias = phone;
                await _dbContext.SaveChangesAsync();
            }
        }

        return RedirectToPage();
    }

    private static CookieOptions BuildLongCookie()
    {
        return new CookieOptions
        {
            Expires = DateTimeOffset.UtcNow.AddYears(10),
            Path = "/",
            IsEssential = true
        };
    }

    private static List<string> ParseCookieList(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            return new List<string>();
        }

        return raw
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Trim())
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct()
            .ToList();
    }
}
