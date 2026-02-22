using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;

namespace GoMad.Pages;

public class MainModel : PageModel
{
    public string Phone { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;

    public void OnGet()
    {
        Phone = Request.Cookies["GoMad_Phone"] ?? string.Empty;
        PostalCode = Request.Cookies["GoMad_PostalCode"] ?? string.Empty;
        Street = Request.Cookies["GoMad_Street"] ?? string.Empty;
    }
}
