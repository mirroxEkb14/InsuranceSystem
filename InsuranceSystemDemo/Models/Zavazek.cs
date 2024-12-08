using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace InsuranceSystemDemo.Models;

[Table("ZAVAZKY")]
public class Zavazek
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("ID_ZAVAZKY")]
    public int IdZavazky { get; set; }
    [Column("SUMA_ZAVAZKY")]
    [Required]
    public decimal SumaZavazky { get; set; }
    [Column("DATA_VZNIKU")]
    [Required]
    public DateTime DataVzniku { get; set; }
    [Column("DATA_SPLATNISTI")]
    [Required]
    public DateTime DataSplatnisti { get; set; }

    // Navigation property
    [Column("POHLEDAVKA_ID_POHLEDAVKY")]
    [Required]
    public int PohledavkaIdPohledavky { get; set; }

    // Object property
    [ForeignKey("PohledavkaIdPohledavky")]
    public virtual Pohledavka? Pohledavka { get; set; } = null!;
}
