using System.ComponentModel.DataAnnotations;

namespace InsuranceSystemDemo.Models;

public class LogAkce
{
    [Key]  
    public int IdLog { get; set; }
    public string? TypAkce { get; set; }
    public DateTime DatumAkce { get; set; }
    public string? UzivatelId { get; set; }
    public string? Popis { get; set; }
}
