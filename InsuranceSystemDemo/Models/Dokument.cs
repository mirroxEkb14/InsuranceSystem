using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InsuranceSystemDemo.Models;

[Table("DOKUMENT")]
public class Dokument
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("ID")]
    public int Id { get; set; }
    [Column("NAZEV")]
    [MaxLength(255)]
    public string? Nazev { get; set; }
    [Column("TYP_SOUBORU")]
    [MaxLength(50)]
    public string? TypSouboru { get; set; }
    [Column("PRIPONA")]
    [MaxLength(10)]
    public string? Pripona { get; set; }
    [Column("DATUM_NAHRANI")]
    public DateTime? DatumNahrani { get; set; }
    [Column("DATUM_MODIFIKACE")]
    public DateTime? DatumModifikace { get; set; }
    [Column("OBSAH")]
    public byte[]? Obsah { get; set; }
    [Column("UZIVATEL")]
    [MaxLength(100)]
    public string? Uzivatel { get; set; }
}
