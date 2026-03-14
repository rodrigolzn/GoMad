using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoMad.Models;

[Table("voluntarios")]
public class Voluntario
{
    [Key]
    [Column("id_voluntario")]
    public int IdVoluntario { get; set; }

    [Required]
    [MaxLength(70)]
    [Column("correo")]
    public string Correo { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    [Column("contrasena")]
    public string ContrasenaHash { get; set; } = string.Empty;

    [Required]
    [MaxLength(6)]
    [Column("clave_zona")]
    public string ClaveZona { get; set; } = string.Empty;
}
