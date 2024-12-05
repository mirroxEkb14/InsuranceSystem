using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InsuranceSystemDemo.Models;

[Table("ADRESA")]  
public class Adresa
{
    [Key]
    [Column("ID_ADRESA")] 
    public int IdAdresa { get; set; }
    [Column("ULICE")] 
    public string? Ulice { get; set; }
    [Column("MESTO")]  
    public string? Mesto { get; set; }
    [Column("STAT")]  
    public string? Stat { get; set; }
    [Column("CISLO_POPISNE")]  
    public string? CisloPopisne { get; set; }
    [Column("PSC")] 
    public string? PSC { get; set; }
}
