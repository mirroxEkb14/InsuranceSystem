using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InsuranceSystemDemo.Models;

[Table("KLIENT")]
public class Klient
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("ID_KLIENTU")]
    public int IdKlientu { get; set; }

    [Column("JMENO")]
    public string? Jmeno { get; set; }

    [Column("PRIJMENI")]
    public string? Prijmeni { get; set; }

    [Column("EMAIL")]
    public string? Email { get; set; }

    [Column("TELEFON")]
    public string? Telefon { get; set; }

    [Column("ADRESA_ID_ADRESA")]
    public int? AdresaId { get; set; }

    [Column("DATUM_NAROZENI")]
    public DateTime? DatumNarozeni { get; set; }

    [ForeignKey("AdresaId")]
    public Adresa? Adresa { get; set; }

    [NotMapped]
    public string FullName => $"{Jmeno} {Prijmeni}";
}
