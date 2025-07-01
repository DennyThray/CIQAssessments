using ProviderBillingBlazor.Components;
using Microsoft.EntityFrameworkCore;
using BillingData.DAL.Context;
using ApexCharts;
using BillingData.DAL.Services;

var builder = WebApplication.CreateBuilder(args);

var config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .Build();

var dbRelativePath = config["Paths:Database"];
var dbFullPath = Path.GetFullPath(dbRelativePath!, AppContext.BaseDirectory);
var connectionString = $"Data Source={dbFullPath}";

Console.WriteLine($"Connection String = {connectionString}");

builder.Services.AddDbContext<BillingContext>(options =>
    {
        try
        {
            options.UseSqlite(connectionString);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Failed to configure DbContext: {ex.Message}");
        }
    });

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddApexCharts();
builder.Services.AddScoped<ProviderService>();
builder.Services.AddSingleton<NationalAveragesService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<BillingContext>();
    var avgService = scope.ServiceProvider.GetRequiredService<NationalAveragesService>();
    await avgService.LoadAsync(db);
}

app.Run();
