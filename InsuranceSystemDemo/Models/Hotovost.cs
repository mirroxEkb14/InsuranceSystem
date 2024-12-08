using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace InsuranceSystemDemo.Models;

[Table("HOTOVOST")]
public class Hotovost
{
    // Navigation property
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    [Column("ID_PLATBY")]
    public int IdPlatby { get; set; }

    // Object property
    [ForeignKey("IdPlatby")]
    public virtual Platba? Platba { get; set; } = null!;

    [Column("PRIJATO")]
    public decimal Prijato { get; set; }
    [Column("VRACENO")]
    public decimal Vraceno { get; set; }
}
