using System;
using System.Collections.Generic;
using System.Linq;
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

    public class Order
    {
        public string Street { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        // "Comida" o "Farmacia"
        public string Type { get; set; } = "Comida";
    }

    public string VolunteerEmail { get; set; } = string.Empty;

    public List<Order> Orders { get; set; } = new();

    public Dictionary<string, List<Order>> RouteGroups { get; set; } = new();

    public IActionResult OnGet()
    {
        // Si no está logueado, redirigir al login
        if (!Request.Cookies.TryGetValue("VolunteerLoggedIn", out var val) || val != "1")
        {
            return RedirectToPage("VolunteerLogin");
        }

        VolunteerEmail = Request.Cookies["VolunteerEmail"] ?? "voluntario@gomad.es";

        // Example pending orders: only calle, código postal y teléfono, tipo restringido a Comida/Farmacia
        // Use the requested San Esteban de Gormaz example: two houses and Eroski City supermarket.
        Orders = new List<Order>
        {
            new Order{ Street = "Calle de las Cuestas 3", PostalCode = "42330", Phone = "+34 600111222", Type = "Comida", Lat=41.4980, Lng=-3.0300 },
            new Order{ Street = "Calle Mayor 12", PostalCode = "42330", Phone = "+34 600333444", Type = "Comida", Lat=41.4990, Lng=-3.0285 }
        };

        // Supermarket location: Eroski City, P.º de las Acacias, 11, 42330 San Esteban de Gormaz
        var supermarket = new Order{ Street = "P.º de las Acacias 11 (Eroski City)", PostalCode = "42330", Phone = "", Type = "Comida", Lat=41.4975, Lng=-3.0270 };

        // Decide best order between the two houses to minimize travel: try both permutations
        double Dist(Order a, Order b)
        {
            return Haversine(a.Lat, a.Lng, b.Lat, b.Lng);
        }

        var a = Orders[0];
        var b = Orders[1];

        var distAthenBthenS = Dist(a,b) + Dist(b, supermarket);
        var distBthenAthenS = Dist(b,a) + Dist(a, supermarket);

        List<Order> routeOrdered;
        if (distAthenBthenS <= distBthenAthenS)
        {
            routeOrdered = new List<Order>{ a, b, supermarket };
        }
        else
        {
            routeOrdered = new List<Order>{ b, a, supermarket };
        }

        // Expose route as a single RouteGroups entry for UI rendering
        RouteGroups = new Dictionary<string, List<Order>>{
            { "Ruta optimizada (casas → Eroski)", routeOrdered }
        };

        // Build Google Maps directions URL: origin = first, destination = last, waypoints = middle entries
        string Encode(string s) => Uri.EscapeDataString(s);
        var origin = routeOrdered.First();
        var destination = routeOrdered.Last();
        var mid = routeOrdered.Skip(1).Take(routeOrdered.Count-2).ToList();
        // If there is one middle point, include it as waypoint; otherwise none.
        var waypoints = string.Join("|", mid.Select(x => Encode($"{x.Street}, {x.PostalCode} San Esteban de Gormaz")));
        var originStr = Encode($"{origin.Street}, {origin.PostalCode} San Esteban de Gormaz");
        var destStr = Encode($"{destination.Street}, {destination.PostalCode} San Esteban de Gormaz");
        string mapsUrl;
        if (!string.IsNullOrEmpty(waypoints))
        {
            mapsUrl = $"https://www.google.com/maps/dir/?api=1&origin={originStr}&destination={destStr}&travelmode=driving&waypoints={waypoints}";
        }
        else
        {
            mapsUrl = $"https://www.google.com/maps/dir/?api=1&origin={originStr}&destination={destStr}&travelmode=driving";
        }

        // Store URL in TempData for view (or ViewData)
        ViewData["GoogleMapsRouteUrl"] = mapsUrl;

        return Page();
    }

    public IActionResult OnPostLogout()
    {
        Response.Cookies.Delete("VolunteerLoggedIn");
        Response.Cookies.Delete("VolunteerEmail");
        return RedirectToPage("Index");
    }

    private static string GetSupermarketForPostalCode(string postal)
    {
        if (string.IsNullOrWhiteSpace(postal)) return "Supermercado Local";
        if (postal.StartsWith("28")) return "Mercadona";
        if (postal.StartsWith("08")) return "Carrefour";
        if (postal.StartsWith("46")) return "Consum";
        return "Supermercado Local";
    }

    private static double Haversine(double lat1, double lon1, double lat2, double lon2)
    {
        double R = 6371e3; // metres
        var phi1 = lat1 * Math.PI / 180.0;
        var phi2 = lat2 * Math.PI / 180.0;
        var dphi = (lat2-lat1) * Math.PI / 180.0;
        var dlambda = (lon2-lon1) * Math.PI / 180.0;
        var a = Math.Sin(dphi/2)*Math.Sin(dphi/2) + Math.Cos(phi1)*Math.Cos(phi2)*Math.Sin(dlambda/2)*Math.Sin(dlambda/2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1-a));
        var d = R * c;
        return d; // meters
    }
}
