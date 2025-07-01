using BillingData.DAL.Context;
using Microsoft.EntityFrameworkCore;

public static class BillingDbFactory
{
    public static BillingContext Create(string connectionString)
    {
        var options = new DbContextOptionsBuilder<BillingContext>()
            .UseSqlite(connectionString)
            .Options;

        return new BillingContext(options);
    }
}