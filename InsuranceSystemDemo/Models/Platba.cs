using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace InsuranceSystemDemo.Models;

[Table("PLATBA")]
public class Platba
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("ID_PLATBY")]
    public int IdPlatby { get; set; }
    [Column("DATUM_PLATBY")]
    public DateTime DatumPlatby { get; set; }
    [Column("SUMA_PLATBY")]
    public decimal SumaPlatby { get; set; }
    [Column("TYP_PLATBY")]
    public string? TypPlatby { get; set; }

    // Navigation properties
    [Column("KLIENT_ID_KLIENTU")]
    public int KlientIdKlientu { get; set; }
    [Column("POJISTNASMLOUVA_ID_POJISTKY")]
    public int PojistnaSmlouvaIdPojistky { get; set; }

    // Object properties
    [ForeignKey("KlientIdKlientu")]
    public virtual Klient? Klient { get; set; }
    [ForeignKey("PojistnaSmlouvaIdPojistky")]
    public virtual PojistnaSmlouva? PojistnaSmlouva { get; set; }

    [NotMapped]
    public string Name => $"{SumaPlatby}, {DatumPlatby:dd.MM.yyyy}, {TypPlatby}";
}
