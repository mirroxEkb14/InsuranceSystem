namespace InsuranceSystemDemo.Models;

public class Zamestnanec : Osoba
{
    public int IdZamestnance { get; set; }
    public string? Role { get; set; }
    public Adresa? Adresa { get; set; }
}
