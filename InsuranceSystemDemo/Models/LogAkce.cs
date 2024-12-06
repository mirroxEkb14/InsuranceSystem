using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InsuranceSystemDemo.Models;

[Table("LOGY")]
public class LogAkce
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("ID")]
    public int Id { get; set; }
    [Column("UZIVATEL_ID")]
    [Required]
    public int UzivatelId { get; set; }
    [Column("TYP_AKCE")]
    [Required]
    [MaxLength(50)]
    public string TypAkce { get; set; }
    [Column("DATUM_AKCE")]
    public DateTime? DatumAkce { get; set; }
    [Column("POPIS")]
    [MaxLength(255)]
    public string? Popis { get; set; }
}
