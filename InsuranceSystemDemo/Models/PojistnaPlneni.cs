using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace InsuranceSystemDemo.Models;

[Table("POJISTNAPLNENI")]
public class PojistnaPlneni
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("ID_PLNENI")]
    public int IdPlneni { get; set; }
    [Column("SUMA_PLNENI")]
    [DataType("decimal(10,2)")]
    public decimal SumaPlneni { get; set; }

    // Navigation properties
    [Column("POJISTNASMOULVA_ID_POJISTKY")]
    public int PojistnaSmlouvaId { get; set; }
    [Column("ZAVAZKY_ID_ZAVAZKY")]
    public int ZavazkyId { get; set; }

    // Object properties
    [ForeignKey("PojistnaSmlouvaId")]
    public virtual PojistnaSmlouva? PojistnaSmlouva { get; set; }
    [ForeignKey("ZavazkyId")]
    public virtual Zavazek? Zavazky { get; set; }
}
