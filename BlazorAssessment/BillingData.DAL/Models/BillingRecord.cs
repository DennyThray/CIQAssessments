using System.ComponentModel.DataAnnotations.Schema;

namespace BillingData.DAL.Models;

public class BillingRecord
{
    public int Id { get; set; }

    public string NPI { get; set; } = null!;
    public string HCPCSCode { get; set; } = null!;
    public string HCPCSDescription { get; set; } = null!;
    public string PlaceOfService { get; set; } = null!;
    public int NumberOfServices { get; set; }
    public decimal TotalMedicarePayment { get; set; }

    public Provider? Provider { get; set; }

    [NotMapped]
    public decimal? NationalAverage { get; set; }
}
