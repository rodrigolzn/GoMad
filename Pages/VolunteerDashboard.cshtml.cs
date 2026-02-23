using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GoMad.Pages;

public class VolunteerDashboardModel : PageModel
{
    private readonly ILogger<VolunteerDashboardModel> _logger;

    public VolunteerDashboardModel(ILogger<VolunteerDashboardModel> logger)
    {
        _logger = logger;
    }

    public string VolunteerEmail { get; set; } = string.Empty;

    public IActionResult OnGet()
    {
        // Si no está logueado, redirigir al login
        if (!Request.Cookies.TryGetValue("VolunteerLoggedIn", out var val) || val != "1")
        {
            return RedirectToPage("VolunteerLogin");
        }

        VolunteerEmail = Request.Cookies["VolunteerEmail"] ?? "voluntario@gomad.es";
        return Page();
    }

    public IActionResult OnPostLogout()
    {
        Response.Cookies.Delete("VolunteerLoggedIn");
        Response.Cookies.Delete("VolunteerEmail");
        return RedirectToPage("Index");
    }
}
