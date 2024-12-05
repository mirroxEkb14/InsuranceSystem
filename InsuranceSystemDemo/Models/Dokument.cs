namespace InsuranceSystemDemo.Models;

public class Dokument
{
    public int Id { get; set; }              
    public string? Nazev { get; set; }       
    public string? TypSouboru { get; set; }  
    public string? Pripona { get; set; }      
    public DateTime DatumNahrani { get; set; } 
    public DateTime DatumModifikace { get; set; } 
    public byte[]? Obsah { get; set; }       
    public string? Uzivatel { get; set; }     
}
