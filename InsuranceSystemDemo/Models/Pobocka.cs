using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace InsuranceSystemDemo.Models;

[Table("POBOCKY")]
public class Pobocka
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("ID_POBOCKY")]
    public int IdPobocky { get; set; }
    [Column("NAZEV")]
    [StringLength(100)]
    public string Nazev { get; set; } = string.Empty;
    [Column("TELEFON")]
    [StringLength(13)]
    public string Telefon { get; set; } = string.Empty;
    [Column("ADRESA_ID_ADRESA")]
    public int AdresaId { get; set; }

    // Navigation property
    [ForeignKey("AdresaId")]
    public virtual Adresa? Adresa { get; set; }
}
