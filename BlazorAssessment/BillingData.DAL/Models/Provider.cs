namespace BillingData.DAL.Models;

public class Provider
{
    public string NPI { get; set; } = null!;
    public string ProviderName { get; set; } = null!;
    public string Specialty { get; set; } = null!;
    public string State { get; set; } = null!;

    public ICollection<BillingRecord> BillingRecords { get; set; } = new List<BillingRecord>();
}
