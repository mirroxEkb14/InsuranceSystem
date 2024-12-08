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
    [Column("DATIM_KONCE")]
    public DateTime DatumKonce { get; set; }

    // Navigation property
    [Column("POJISTNASMLOUVA_ID_POJISTKY")]
    public int PojistnaSmlouvaId { get; set; }

    // Object property
    [ForeignKey("PojistnaSmlouvaId")]
    public virtual PojistnaSmlouva? PojistnaSmlouva { get; set; }

    [NotMapped]
    public string Name => $"Amount: {SumaPohledavky}, Dates: {DatumZacatku:dd.MM.yyyy}-{DatumKonce:dd.MM.yyyy}";
}
