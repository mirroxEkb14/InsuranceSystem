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
    [Column("POJISTNASMLOUVA_ID_POJISTKY")]
    public int PojistnaSmlouvaIdPojistky { get; set; }
    [Column("ZAVAZKY_ID_ZAVAZKY")]
    public int ZavazkyIdZavazky { get; set; }

    // Object properties
    [ForeignKey("PojistnaSmlouvaIdPojistky")]
    public virtual PojistnaSmlouva? PojistnaSmlouva { get; set; }
    [ForeignKey("ZavazkyIdZavazky")]
    public virtual Zavazek? Zavazky { get; set; }
}
