using DataIngestionConsole.Services;

string projectDir = Directory.GetCurrentDirectory();
string inputDir = Path.Combine(projectDir, "Input");
string outputDir = Path.Combine(projectDir, "Output");

Directory.CreateDirectory(inputDir);
Directory.CreateDirectory(outputDir);

// Step 1: Get CSV file (either from arg or latest in Input/)
string? inputCsv = args.Length > 0 ? args[0] : 
    Directory.EnumerateFiles(inputDir, "*.csv").OrderByDescending(File.GetLastWriteTime).FirstOrDefault();

if (inputCsv is null || !File.Exists(inputCsv))
{
    Console.WriteLine($"❌ No CSV file found.");
    return;
}

string outputDb = args.Length > 1 ? args[1] : Path.Combine(outputDir, "billing.db");

Directory.CreateDirectory(Path.GetDirectoryName(outputDb)!);

Console.WriteLine($"📄 Using CSV: {inputCsv}");

// Step 2: Build output path
var dbFileName = "billing.db";
var dbOutputPath = Path.Combine(outputDir, dbFileName);
var connectionString = $"Data Source={dbOutputPath}";

Console.WriteLine($"💾 Writing SQLite DB to: {dbOutputPath}");

// Step 3: Ingest
using var db = BillingDbFactory.Create(connectionString);
await CsvIngestor.IngestAsync(inputCsv, db);

// Step 4: Copy to /bin output folder
var outputBinDir = Path.Combine(AppContext.BaseDirectory, "../../../..", "BillingData.DAL", "Data");
Directory.CreateDirectory(outputBinDir);

var finalPath = Path.Combine(outputBinDir, dbFileName);
File.Copy(dbOutputPath, finalPath, overwrite: true);
Console.WriteLine($"📤 Copied DB to: {finalPath}");
Console.WriteLine("✅ Done.");

Console.WriteLine("🎯 If you're using the Blazor web app, copy:");
Console.WriteLine($"  {outputDb}");
Console.WriteLine("to:");
Console.WriteLine("  BillingData.DAL/Data/billing.db");
Console.WriteLine("Afterwards, clean and build the solution.");