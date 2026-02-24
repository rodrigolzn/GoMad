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

    /// <summary>
    /// Representa un pedido de entrega a domicilio.
    /// Type puede ser "Comida" (se compra en Eroski) o "Farmacia" (se recoge en Farmacia Diego Errazquin).
    /// </summary>
    public class Order
    {
        public string Street { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public double Lat { get; set; } = 0.0;
        public double Lng { get; set; } = 0.0;
        /// <summary>"Comida" o "Farmacia"</summary>
        public string Type { get; set; } = "Comida";
        /// <summary>Nombre del establecimiento donde se compra el pedido.</summary>
        public string BuyAt { get; set; } = string.Empty;
        /// <summary>Items pedidos (descripción libre).</summary>
        public string Items { get; set; } = string.Empty;
        /// <summary>Nombre del beneficiario.</summary>
        public string Recipient { get; set; } = string.Empty;
        /// <summary>True cuando este nodo es un establecimiento de compra, no una entrega.</summary>
        public bool IsStore { get; set; } = false;
        /// <summary>Emoji / icono para mostrar en la UI.</summary>
        public string Icon => Type == "Farmacia" ? "💊" : "🛒";
    }

    public string VolunteerEmail { get; set; } = string.Empty;

    /// <summary>Solo pedidos de entrega (no tiendas).</summary>
    public List<Order> Orders { get; set; } = new();

    /// <summary>Ruta completa optimizada: tiendas primero, luego casas.</summary>
    public List<Order> OptimalRoute { get; set; } = new();

    /// <summary>URL de Google Maps con toda la ruta.</summary>
    public string GoogleMapsRouteUrl { get; set; } = string.Empty;

    public IActionResult OnGet()
    {
        if (!Request.Cookies.TryGetValue("VolunteerLoggedIn", out var val) || val != "1")
            return RedirectToPage("VolunteerLogin");

        VolunteerEmail = Request.Cookies["VolunteerEmail"] ?? "voluntario@gomad.es";

        // ── Pedidos de hoy ──────────────────────────────────────────────
        Orders = new List<Order>
        {
            new Order
            {
                Recipient  = "Carmen Rodríguez",
                Street     = "Calle de las Cuestas 3",
                PostalCode = "42330",
                Phone      = "+34 600 111 222",
                Type       = "Comida",
                BuyAt      = "Eroski City — P.º de las Acacias 11",
                Items      = "Leche (x2), Pan de molde, Fruta variada, Yogures (x4)",
                Lat        = 41.5762,
                Lng        = -3.2145
            },
            new Order
            {
                Recipient  = "Luis Fernández",
                Street     = "Calle Mayor 12",
                PostalCode = "42330",
                Phone      = "+34 600 333 444",
                Type       = "Farmacia",
                BuyAt      = "Farmacia Diego Errazquin — C. Escuelas",
                Items      = "Omeprazol 20 mg (x1 caja), Paracetamol 1 g (x2 cajas), Vitamina D",
                Lat        = 41.5759,
                Lng        = -3.2115
            }
        };

        // ── Nodos tienda ────────────────────────────────────────────────
        var eroski = new Order
        {
            Recipient  = "Eroski City",
            Street     = "P.º de las Acacias 11",
            PostalCode = "42330",
            Phone      = "+34 975 350 XXX",
            Type       = "Comida",
            BuyAt      = "Eroski City",
            Items      = "Comprar pedidos de Comida",
            IsStore    = true,
            Lat        = 41.5748,
            Lng        = -3.2122
        };

        var farmacia = new Order
        {
            Recipient  = "Farmacia Diego Errazquin",
            Street     = "C. Escuelas",
            PostalCode = "42330",
            Phone      = "+34 975 350 YYY",
            Type       = "Farmacia",
            BuyAt      = "Farmacia Diego Errazquin",
            Items      = "Recoger pedidos de Farmacia",
            IsStore    = true,
            Lat        = 41.5756,
            Lng        = -3.2135
        };

        // ── Optimización de ruta: tiendas → casas ───────────────────────
        // Sólo incluimos una tienda si hay al menos un pedido de ese tipo.
        var needsEroski   = Orders.Any(o => o.Type == "Comida");
        var needsFarmacia = Orders.Any(o => o.Type == "Farmacia");

        var stores  = new List<Order>();
        if (needsEroski)   stores.Add(eroski);
        if (needsFarmacia) stores.Add(farmacia);

        var houses = Orders.ToList();

        // Probamos todas las permutaciones: orden de tiendas × orden de casas
        var storePerms = Permutations(stores);
        var housePerms = Permutations(houses);

        List<Order>? bestRoute = null;
        double bestDist = double.MaxValue;

        foreach (var sp in storePerms)
        {
            foreach (var hp in housePerms)
            {
                var candidate = sp.Concat(hp).ToList();
                double d = TotalDistance(candidate);
                if (d < bestDist)
                {
                    bestDist  = d;
                    bestRoute = candidate;
                }
            }
        }

        OptimalRoute = bestRoute ?? stores.Concat(houses).ToList();

        // ── URL Google Maps ─────────────────────────────────────────────
        GoogleMapsRouteUrl = BuildGoogleMapsUrl(OptimalRoute);

        return Page();
    }

    public IActionResult OnPostLogout()
    {
        Response.Cookies.Delete("VolunteerLoggedIn");
        Response.Cookies.Delete("VolunteerEmail");
        return RedirectToPage("Index");
    }

    // ── Helpers ─────────────────────────────────────────────────────────

    private static double TotalDistance(List<Order> route)
    {
        double total = 0;
        for (int i = 0; i < route.Count - 1; i++)
            total += Haversine(route[i].Lat, route[i].Lng, route[i + 1].Lat, route[i + 1].Lng);
        return total;
    }

    private static IEnumerable<List<T>> Permutations<T>(List<T> items)
    {
        if (items.Count <= 1) { yield return items.ToList(); yield break; }
        for (int i = 0; i < items.Count; i++)
        {
            var rest = items.Where((_, idx) => idx != i).ToList();
            foreach (var perm in Permutations(rest))
            {
                perm.Insert(0, items[i]);
                yield return perm;
            }
        }
    }

    private static string BuildGoogleMapsUrl(List<Order> route)
    {
        if (route.Count == 0) return "#";

        string Encode(Order o) => Uri.EscapeDataString($"{o.Street}, {o.PostalCode} San Esteban de Gormaz, España");

        var origin      = Encode(route.First());
        var destination = Encode(route.Last());
        var waypointList = route.Skip(1).Take(route.Count - 2).Select(Encode).ToList();
        var waypoints   = string.Join("|", waypointList);

        var url = $"https://www.google.com/maps/dir/?api=1&origin={origin}&destination={destination}&travelmode=driving";
        if (!string.IsNullOrEmpty(waypoints))
            url += $"&waypoints={waypoints}";
        return url;
    }

    private static double Haversine(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371e3;
        var phi1    = lat1 * Math.PI / 180.0;
        var phi2    = lat2 * Math.PI / 180.0;
        var dphi    = (lat2 - lat1) * Math.PI / 180.0;
        var dlambda = (lon2 - lon1) * Math.PI / 180.0;
        var a = Math.Sin(dphi / 2) * Math.Sin(dphi / 2)
              + Math.Cos(phi1) * Math.Cos(phi2)
              * Math.Sin(dlambda / 2) * Math.Sin(dlambda / 2);
        return R * 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
    }
}
