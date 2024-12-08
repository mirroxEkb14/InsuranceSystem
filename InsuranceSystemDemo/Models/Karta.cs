using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace InsuranceSystemDemo.Models;

[Table("KARTA")]
public class Karta
{
    // Navigation property
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    [Column("ID_PLATBY")]
    public int IdPlatby { get; set; }

    // Object property
    [ForeignKey("IdPlatby")]
    public virtual Platba? Platba { get; set; } = null!;

    [Column("CISLO_KARTY")]
    public decimal CisloKarty { get; set; }
    [Column("CISLO_UCTU")]
    public decimal CisloUctu { get; set; }
}
