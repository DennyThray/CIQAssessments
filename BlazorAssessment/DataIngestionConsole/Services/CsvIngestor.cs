using System.Globalization;
using BillingData.DAL.Context;
using BillingData.DAL.Models;
using CsvHelper;
using CsvHelper.Configuration;

namespace DataIngestionConsole.Services;

public static class CsvIngestor
{
    public static async Task IngestAsync(string csvPath, BillingContext db)
    {
        Console.WriteLine("📊 Parsing CSV...");

        using var reader = new StreamReader(csvPath);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HeaderValidated = null,
            MissingFieldFound = null,
            BadDataFound = null,
            PrepareHeaderForMatch = args => args.Header.ToLowerInvariant()
        });

        var providers = new Dictionary<string, Provider>();
        var billingRecords = new List<BillingRecord>();

        await foreach (var row in csv.GetRecordsAsync<dynamic>())
        {
            string npi = row.rndrng_npi;
            if (string.IsNullOrWhiteSpace(npi)) continue;

            if (!providers.ContainsKey(npi))
            {
                providers[npi] = new Provider
                {
                    NPI = npi,
                    ProviderName = row.rndrng_prvdr_last_org_name ?? "Unknown",
                    Specialty = row.rndrng_prvdr_type ?? "Unknown",
                    State = row.rndrng_prvdr_state_abrvtn ?? "Unknown"
                };
            }

            int services = 0;
            decimal payment = 0;

            if (int.TryParse(row.tot_srvcs, out services) &&
                decimal.TryParse(row.avg_mdcr_pymt_amt, out payment))
            {
                billingRecords.Add(new BillingRecord
                {
                    NPI = npi,
                    HCPCSCode = row.hcpcs_cd ?? "UNKNOWN",
                    HCPCSDescription = row.hcpcs_desc ?? "N/A",
                    PlaceOfService = row.place_of_srvc ?? "N/A",
                    NumberOfServices = services,
                    TotalMedicarePayment = payment
                });
            }
        }

        Console.WriteLine($"✅ Parsed {providers.Count} providers, {billingRecords.Count} billing records");

        Console.WriteLine("💾 Writing to database...");
        await db.Database.EnsureDeletedAsync();
        await db.Database.EnsureCreatedAsync();

        await db.RemoveIndexesAsync();

        db.ChangeTracker.AutoDetectChangesEnabled = false;

        await db.Providers.AddRangeAsync(providers.Values);
        await db.SaveChangesAsync();

        const int batchSize = 20000;
        int total = billingRecords.Count;

        for (int i = 0; i < total; i += batchSize)
        {
            var batch = billingRecords.Skip(i).Take(batchSize);
            await db.BillingRecords.AddRangeAsync(batch);
            await db.SaveChangesAsync();

            // 🟢 Progress tracker
            double percent = (i + batchSize) / (double)total * 100;
            percent = Math.Min(percent, 100);
            Console.Write($"\r🧮 Inserting billing records... {percent:F1}%");
        }
        Console.WriteLine("\n✅ All billing records inserted.");

        Console.WriteLine("🎉 Ingestion complete.");
        await db.CreateIndexesAsync();
    }
}
