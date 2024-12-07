using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InsuranceSystemDemo.Models;

[Table("TYPPOJISTKY")]
public class TypPojistky
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("ID_TYP")]
    public int IdTyp { get; set; }

    [Column("DOSTUPNOST")]
    [Required]
    [StringLength(1)]
    public string Dostupnost { get; set; } = string.Empty;

    [Column("PODMINKY")]
    [Required]
    [StringLength(100)]
    public string Podminky { get; set; } = string.Empty;

    [Column("POPIS")]
    [StringLength(100)]
    public string? Popis { get; set; }

    [Column("MAXIMALNE_KRYTI")]
    [Required]
    public decimal MaximalneKryti { get; set; }

    [Column("MINIMALNE_KRYTI")]
    [Required]
    public decimal MinimalneKryti { get; set; }

    [Column("DATIM_ZACATKU")]
    [Required]
    public DateTime DatumZacatku { get; set; }

    [NotMapped]
    public string Name => string.IsNullOrWhiteSpace(Popis) ? "No Description" : Popis;
}
