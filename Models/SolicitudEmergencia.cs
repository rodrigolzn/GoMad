using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoMad.Models;

[Table("solicitudes_emergencia")]
public class SolicitudEmergencia
{
    [Key]
    [Column("id_solicitud_emergencia")]
    public int IdSolicitudEmergencia { get; set; }

    [Column("usuario_id")]
    public int UsuarioId { get; set; }

    [Required]
    [MaxLength(20)]
    [Column("telefono_solicitante")]
    public string TelefonoSolicitante { get; set; } = string.Empty;

    [Required]
    [Column("aceptada")]
    public bool Aceptada { get; set; }

    [Required]
    [Column("creada_en")]
    public DateTime CreadaEn { get; set; } = DateTime.UtcNow;

    public Usuario? Usuario { get; set; }
}