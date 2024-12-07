using InsuranceSystemDemo.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

[Table("ZAMESTNANEC")]
public class Zamestnanec
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("ID_ZAMESTNANCE")]
    public int IdZamestnance { get; set; }

    [Column("ROLE")]
    [Required]
    [StringLength(4000)]
    public string Role { get; set; } = string.Empty;

    [Column("POBOCKY_ID_POBOCKY")]
    [Required]
    public int PobockyIdPobocky { get; set; }

    [Column("JMENO")]
    [Required]
    [StringLength(4000)]
    public string Jmeno { get; set; } = string.Empty;

    [Column("PRIJMENI")]
    [Required]
    [StringLength(4000)]
    public string Prijmeni { get; set; } = string.Empty;

    [Column("EMAIL")]
    [StringLength(50)]
    public string? Email { get; set; }

    [Column("TELEFON")]
    [Required]
    public long Telefon { get; set; }

    [Column("ADRESA_ID_ADRESA")]
    [Required]
    public int AdresaIdAdresa { get; set; }

    [Column("POPIS")]
    public string? Popis { get; set; }

    // Связь с Pobocka
    [ForeignKey("PobockyIdPobocky")]
    public virtual Pobocka Pobocka { get; set; } = null!;

    // Связь с Adresa
    [ForeignKey("AdresaIdAdresa")]
    public virtual Adresa Adresa { get; set; } = null!;
}
