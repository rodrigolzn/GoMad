using GoMad.Mobile.Models;

namespace GoMad.Mobile;

public sealed class AppState
{
    public OnboardingInput Onboarding { get; } = new();
    public bool OnboardingCompleted { get; private set; }
    public int UserId { get; private set; } = 1;
    public string UserPhone { get; private set; } = string.Empty;
    public string UserPostalCode { get; private set; } = string.Empty;
    public string UserStreet { get; private set; } = string.Empty;
    public bool MicGranted { get; set; }
    public bool MicOn { get; set; }
    public string? HelpMessage { get; set; }

    public VolunteerLoginInput VolunteerLogin { get; } = new();
    public bool VolunteerLoggedIn { get; private set; }
    public string VolunteerEmail { get; private set; } = string.Empty;

    public List<string> EmergencyRequests { get; } = new();
    public List<string> AcceptedContacts { get; } = new();

    public List<VolunteerOrder> PendingOrders { get; } = new();
    public List<VolunteerOrder> RouteStops { get; } = new();
    public string GoogleMapsRouteUrl { get; private set; } = "#";

    public AppState()
    {
        PendingOrders.AddRange(new[]
        {
            new VolunteerOrder
            {
                Recipient = "Carmen Rodríguez",
                Street = "Calle de las Cuestas 3",
                PostalCode = "42330",
                Phone = "+34 600 111 222",
                Type = "Comida",
                BuyAt = "Eroski City — P.º de las Acacias 11",
                Items = "Leche (x2), Pan de molde, Fruta variada, Yogures (x4)",
                Lat = 41.5762,
                Lng = -3.2145
            },
            new VolunteerOrder
            {
                Recipient = "Luis Fernández",
                Street = "Calle Mayor 12",
                PostalCode = "42330",
                Phone = "+34 600 333 444",
                Type = "Farmacia",
                BuyAt = "Farmacia Diego Errazquin — C. Escuelas",
                Items = "Omeprazol 20 mg (x1 caja), Paracetamol 1 g (x2 cajas), Vitamina D",
                Lat = 41.5759,
                Lng = -3.2115
            }
        });

        var eroski = new VolunteerOrder
        {
            Recipient = "Eroski City",
            Street = "P.º de las Acacias 11",
            PostalCode = "42330",
            Phone = "+34 975 350 XXX",
            Type = "Comida",
            BuyAt = "Eroski City",
            Items = "Comprar pedidos de Comida",
            IsStore = true,
            Lat = 41.5748,
            Lng = -3.2122
        };

        var farmacia = new VolunteerOrder
        {
            Recipient = "Farmacia Diego Errazquin",
            Street = "C. Escuelas",
            PostalCode = "42330",
            Phone = "+34 975 350 YYY",
            Type = "Farmacia",
            BuyAt = "Farmacia Diego Errazquin",
            Items = "Recoger pedidos de Farmacia",
            IsStore = true,
            Lat = 41.5756,
            Lng = -3.2135
        };

        RouteStops.AddRange(new[]
        {
            eroski,
            farmacia,
            PendingOrders[0],
            PendingOrders[1]
        });

        GoogleMapsRouteUrl = BuildGoogleMapsUrl(RouteStops);
    }

    public void CompleteOnboarding(OnboardingInput input)
    {
        OnboardingCompleted = true;
        UserPhone = string.Concat(input.PhonePrefix.Trim(), input.Phone.Trim());
        UserPostalCode = input.PostalCode.Trim();
        UserStreet = input.Street.Trim();
        Onboarding.Phone = input.Phone;
        Onboarding.PhonePrefix = input.PhonePrefix;
        Onboarding.PostalCode = input.PostalCode;
        Onboarding.Street = input.Street;
        UserId = Math.Max(UserId, 1);
    }

    public void RegisterEmergencyRequest(string fullPhone, string beneficiaryId)
    {
        var request = fullPhone.Trim();
        if (!EmergencyRequests.Contains(request))
        {
            EmergencyRequests.Add(request);
        }
    }

    public void ResolveEmergency(string phone, bool accept)
    {
        EmergencyRequests.RemoveAll(x => x == phone);
        if (accept && !AcceptedContacts.Contains(phone))
        {
            AcceptedContacts.Add(phone);
        }
    }

    public bool TryVolunteerLogin(string email, string password)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();
        var allowedEmails = new HashSet<string>
        {
            "voluntariogm1@gmail.com",
            "voluntariogm2@gmail.com",
            "voluntariogm3@gmail.com",
            "voluntariogm4@gmail.com",
            "voluntariogm5@gmail.com"
        };

        if (!allowedEmails.Contains(normalizedEmail) || password != "1234asdfASDF")
        {
            return false;
        }

        VolunteerLoggedIn = true;
        VolunteerEmail = normalizedEmail;
        return true;
    }

    public void LogoutVolunteer()
    {
        VolunteerLoggedIn = false;
        VolunteerEmail = string.Empty;
    }

    private static string BuildGoogleMapsUrl(List<VolunteerOrder> route)
    {
        if (route.Count == 0)
        {
            return "#";
        }

        var stops = route.Select(o => Uri.EscapeDataString($"{o.Street}, {o.PostalCode} San Esteban de Gormaz, España"));
        return "https://www.google.com/maps/dir/" + string.Join("/", stops);
    }
}