using System.ComponentModel.DataAnnotations;

namespace GoMad.Mobile.Models;

public sealed class OnboardingInput
{
    [Required(ErrorMessage = "Introduce tu teléfono")]
    [Phone]
    public string Phone { get; set; } = string.Empty;

    public string PhonePrefix { get; set; } = "+34";

    [Required(ErrorMessage = "Introduce tu código postal")]
    [RegularExpression(@"^[0-9]{4,5}$", ErrorMessage = "Código postal inválido")]
    public string PostalCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Introduce la calle")]
    public string Street { get; set; } = string.Empty;
}

public sealed class EmergencyContactInput
{
    public string ContactPhonePrefix { get; set; } = "+34";

    [Required(ErrorMessage = "Debes indicar tu teléfono")]
    [Phone(ErrorMessage = "Número de teléfono no válido")]
    public string ContactPhone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Debes indicar el ID del beneficiario")]
    [RegularExpression(@"^[0-9]+$", ErrorMessage = "El ID del beneficiario debe ser numérico")]
    public string BeneficiaryID { get; set; } = string.Empty;
}

public sealed class VolunteerLoginInput
{
    [Required(ErrorMessage = "Introduce tu correo electrónico")]
    [EmailAddress(ErrorMessage = "Correo electrónico no válido")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Introduce tu contraseña")]
    public string Password { get; set; } = string.Empty;
}

public sealed class VolunteerOrder
{
    public string Street { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public double Lat { get; set; }
    public double Lng { get; set; }
    public string Type { get; set; } = "Comida";
    public string BuyAt { get; set; } = string.Empty;
    public string Items { get; set; } = string.Empty;
    public string Recipient { get; set; } = string.Empty;
    public bool IsStore { get; set; }
    public bool IsDelivered { get; set; }
    public string Icon => Type == "Farmacia" ? "💊" : "🛒";
}