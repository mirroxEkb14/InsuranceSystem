using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InsuranceSystemDemo.Models;

[Table("POJISTNASMLOUVA")]
public class PojistnaSmlouva
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("ID_POJISTKY")]
    public int IdPojistky { get; set; }
    [Column("POJISTNA_CASTKA")]
    [DataType("decimal(10,2)")]
    public decimal PojistnaCastka { get; set; }
    [Column("DATUM_ZACATKU_PLAINOSTI")]
    public DateTime DatumZacatkuPlatnosti { get; set; }
    [Column("DATUM_UKONCENI_PLAINOSTI")]
    public DateTime DatumUkonceniPlatnosti { get; set; }
    [Column("DATA_VYSTAVENI")]
    public DateTime DataVystaveni { get; set; }
    [Column("CENA")]
    [DataType("decimal(10,2)")]
    public decimal Cena { get; set; }

    // Navigation properties
    [Column("KLIENT_ID_KLIENTU")]
    public int KlientId { get; set; }
    [Column("POBOCKY_ID_POBOCKY")]
    public int PobockyId { get; set; }
    [Column("TYPPOJISTKY_ID_TYP")]
    public int TypPojistkyId { get; set; }

    // Object properties
    [ForeignKey("KlientId")]
    public virtual Klient? Klient { get; set; }
    [ForeignKey("PobockyId")]
    public virtual Pobocka? Pobocka { get; set; }
    [ForeignKey("TypPojistkyId")]
    public virtual TypPojistky? TypPojistky { get; set; }

    [NotMapped]
    public string Name => $"Amount: {PojistnaCastka}, Dates: {DatumZacatkuPlatnosti:dd.MM.yyyy}-{DatumUkonceniPlatnosti:dd.MM.yyyy}";
}
