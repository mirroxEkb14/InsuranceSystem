using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace InsuranceSystemDemo.Models;

[Table("FAKTURA")]
public class Faktura
{
    // Navigation property
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    [Column("ID_PLATBY")]
    public int IdPlatby { get; set; }

    // Object property
    [ForeignKey("IdPlatby")]
    public virtual Platba? Platba { get; set; } = null!;

    [Column("CISLO_UCTU")]
    public decimal CisloUctu { get; set; }
    [Column("DATUM_SPLATNOSTI")]
    public DateTime DatumSplatnosti { get; set; }
}
