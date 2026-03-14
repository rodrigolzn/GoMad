using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoMad.Models;

[Table("usuarios")]
public class Usuario
{
    [Key]
    [Column("id_usuario")]
    public int IdUsuario { get; set; }

    [Required]
    [MaxLength(20)]
    [Column("telefono")]
    public string Telefono { get; set; } = string.Empty;

    [Required]
    [MaxLength(5)]
    [Column("codigo_postal")]
    public string CodigoPostal { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    [Column("calle")]
    public string Calle { get; set; } = string.Empty;

    [MaxLength(20)]
    [Column("telefono_emergencias")]
    public string? TelefonoEmergencias { get; set; }
}
