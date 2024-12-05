namespace InsuranceSystemDemo.Models;

public class ActivePolicies
{
    public int PolicyId { get; set; }
    public decimal InsuranceAmount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int ClientId { get; set; }
    public string? ClientName { get; set; }
    public int PolicyCount { get; set; }
}

