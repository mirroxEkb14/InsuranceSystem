using System.ComponentModel.DataAnnotations;

namespace InsuranceSystemDemo.Models;

public class PojistnaSmlouva
{
    [Key]
    public int IdPojistky { get; set; }
    public decimal PojistnaCastka { get; set; }
    public DateTime DatumZacatkuPlatnosti { get; set; }
    public DateTime DatumUkonceniPlatnosti { get; set; }
    public DateTime DatumVystaveni { get; set; }
    public decimal Cena { get; set; }
    public Klient? Klient { get; set; }
    public TypPojistky? TypPojistky { get; set; }
}
