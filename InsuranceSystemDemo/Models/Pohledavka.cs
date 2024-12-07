using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace InsuranceSystemDemo.Models;

[Table("POHLEDAVKA")]
public class Pohledavka
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("ID_POHLEDAVKY")]
    public int IdPohledavky { get; set; }
    [Column("SUMA_POHLEDAVKY")]
    [DataType("decimal(10,2)")]
    public decimal SumaPohledavky { get; set; }
    [Column("DATUM_ZACATKU")]
    public DateTime DatumZacatku { get; set; }
    [Column("DATUM_KONCE")]
    public DateTime DatumKonce { get; set; }

    // Navigation properties
    [Column("POJISTNASMOULVA_ID_POJISTKY")]
    public int PojistnaSmlouvaId { get; set; }
    [ForeignKey("PojistnaSmlouvaId")]
    public virtual PojistnaSmlouva? PojistnaSmlouva { get; set; }
}
