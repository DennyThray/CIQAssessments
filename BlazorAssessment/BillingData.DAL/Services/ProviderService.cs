using BillingData.DAL.Context;
using BillingData.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace BillingData.DAL.Services;

public class ProviderService
{
    private readonly BillingContext _db;

    public ProviderService(BillingContext db) => _db = db;

    public async Task<(List<Provider> Results, int TotalCount)> SearchProvidersAsync(
        string search,
        string state,
        string specialty,
        int page,
        int pageSize
    )
    {
        var query = _db.Providers.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(p => p.NPI.Contains(search) || p.ProviderName.Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(state))
            query = query.Where(p => p.State == state);

        if (!string.IsNullOrWhiteSpace(specialty))
            query = query.Where(p => p.Specialty == specialty);

        var count = await query.CountAsync();

        var results = await query
            .OrderBy(p => p.NPI)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (results, count);
    }

    public Task<List<string>> GetDistinctStatesAsync() =>
        _db.Providers.Select(p => p.State).Distinct().OrderBy(s => s).ToListAsync();

    public Task<List<string>> GetDistinctSpecialtiesAsync() =>
        _db.Providers.Select(p => p.Specialty).Distinct().OrderBy(s => s).ToListAsync();

    public Task<List<string>> GetDistinctPlacesOfServiceAsync(string npi) =>
        _db
            .BillingRecords.Where(b => b.NPI == npi)
            .Select(b => b.PlaceOfService)
            .Distinct()
            .OrderBy(x => x)
            .ToListAsync();

    public async Task<List<BillingRecord>> GetTopBillingRecordsAsync(
        string npi,
        string? placeOfService,
        NationalAveragesService avgService
    )
    {
        var query = _db.BillingRecords.Where(b => b.NPI == npi);

        if (!string.IsNullOrWhiteSpace(placeOfService))
        {
            query = query.Where(b => b.PlaceOfService == placeOfService);
        }

        // Work around SQLite's lack of decimal support in ORDER BY
        var records = await query.ToListAsync();

        var top = records.OrderByDescending(b => b.TotalMedicarePayment).Take(10).ToList();

        foreach (var r in top)
        {
            r.NationalAverage = avgService.GetAverage(r.HCPCSCode);
        }

        return top;
    }
}
