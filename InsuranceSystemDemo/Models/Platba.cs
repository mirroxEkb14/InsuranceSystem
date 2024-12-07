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
    [Column("KLIENT_ID_KLIENTU")]
    public int KlientId { get; set; }
    [Column("TYP_PLATBY")]
    public string? TypPlatby { get; set; }

    // Navigation properties
    [Column("POJISTNASMLOUVA_ID")]
    public int PojistnaSmlouvaId { get; set; }
    [ForeignKey("KlientId")]
    public virtual Klient? Klient { get; set; }
    [ForeignKey("PojistnaSmlouvaId")]
    public virtual PojistnaSmlouva? PojistnaSmlouva { get; set; }
}
