using Microsoft.EntityFrameworkCore;
using BillingData.DAL.Models;

namespace BillingData.DAL.Context;

public class BillingContext : DbContext
{
    public BillingContext(DbContextOptions<BillingContext> options)
    : base(options) { }
    public DbSet<Provider> Providers => Set<Provider>();
    public DbSet<BillingRecord> BillingRecords => Set<BillingRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Provider>()
            .HasKey(p => p.NPI);

        modelBuilder.Entity<BillingRecord>()
            .HasIndex(b => b.NPI);
        modelBuilder.Entity<BillingRecord>()
            .HasIndex(b => b.HCPCSCode);
        modelBuilder.Entity<Provider>()
            .HasIndex(p => p.Specialty);
        modelBuilder.Entity<Provider>()
            .HasIndex(p => p.State);

        modelBuilder.Entity<BillingRecord>()
            .HasOne(b => b.Provider)
            .WithMany(p => p.BillingRecords)
            .HasForeignKey(b => b.NPI);
    }

    public async Task RemoveIndexesAsync()
    {
        await Database.ExecuteSqlRawAsync(@"
        DROP INDEX IF EXISTS IX_BillingRecords_NPI;
        DROP INDEX IF EXISTS IX_BillingRecords_HCPCSCode;
        DROP INDEX IF EXISTS IX_Providers_Specialty;
        DROP INDEX IF EXISTS IX_Providers_State;
    ");
    }

    public async Task CreateIndexesAsync()
    {
        await Database.ExecuteSqlRawAsync(@"
        CREATE INDEX IF NOT EXISTS IX_BillingRecords_NPI ON BillingRecords(NPI);
        CREATE INDEX IF NOT EXISTS IX_BillingRecords_HCPCSCode ON BillingRecords(HCPCSCode);
        CREATE INDEX IF NOT EXISTS IX_Providers_Specialty ON Providers(Specialty);
        CREATE INDEX IF NOT EXISTS IX_Providers_State ON Providers(State);
    ");

    }

}
