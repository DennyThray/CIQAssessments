using BillingData.DAL.Context;
using BillingData.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace BillingData.DAL.Services;

public class NationalAveragesService
{
    private Dictionary<string, decimal> _averages = new();

    public async Task LoadAsync(BillingContext db)
    {
        _averages = await db
            .BillingRecords.AsNoTracking()
            .GroupBy(r => r.HCPCSCode)
            .Select(g => new { Code = g.Key, Avg = g.Average(r => r.TotalMedicarePayment) })
            .ToDictionaryAsync(x => x.Code, x => x.Avg);
    }

    public decimal? GetAverage(string code)
    {
        return _averages.TryGetValue(code, out var avg) ? avg : null;
    }
}
