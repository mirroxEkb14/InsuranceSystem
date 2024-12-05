using System.ComponentModel.DataAnnotations;

namespace InsuranceSystemDemo.Models;

public class TypPojistky
{
    [Key]
    public int IdTyp { get; set; }
    public string? Dostupnost { get; set; }
    public string? Podminky { get; set; }
    public string? Popis { get; set; }
    public decimal MinimalneKryti { get; set; }
    public decimal MaximalneKryti { get; set; }
    public DateTime DatumZacatku { get; set; }
}
