# 🧪 Provider Billing Explorer

A two-part coding assessment demonstrating data ingestion and interactive visualization of Medicare billing data using a .NET 8 Blazor Server app and a console ingestion tool.

---

## 📂 Project Structure

```
BlazorAssessment/
├── BillingData.DAL/             # Shared EF Core data access layer
├── DataIngestionConsole/       # Console app for parsing and loading CSV into SQLite
├── ProviderBillingBlazor/      # Blazor Server UI project
```

---

## ⚙️ Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- Visual Studio 2022+ or VS Code

---

## 🔄 Part 1: Data Ingestion Console

### 🧾 What It Does

- Reads a large Medicare billing CSV file
- Normalizes it into two tables: `Provider` and `BillingRecord`
- Bulk-inserts the data into a SQLite database
- Outputs: `billing.db`

### ▶️ Running It

1. Place the `.csv` Medicare file inside:

   ```
   /DataIngestionConsole/Input/
   ```

2. Build & run the console app:

   ```bash
   cd DataIngestionConsole
   dotnet run
   ```

3. Output database will be placed at:

   ```
   /DataIngestionConsole/Output/billing.db
   ```

4. Manually copy that file into:

   ```
   /BillingData.DAL/Data/billing.db
   ```

---

## 🌐 Part 2: Blazor Server App

### 🚀 What It Does

- Lists Medicare providers with filtering, search, and paging
- Shows provider billing details for top 10 HCPCS codes
- Includes:
  - Place of Service filtering
  - Bar chart visualization using ApexCharts
  - National average comparison
  - CSV export

### ▶️ Running It

1. Make sure the database exists at:

   ```
   BillingData.DAL/Data/billing.db
   ```

2. (Optional) if already built, clean and build the app:

   ```bash
   cd ProviderBillingBlazor
   dotnet clean
   dotnet build
   ```

3. Launch the app:

   ```bash
   cd ProviderBillingBlazor
   dotnet run
   ```

4. Visit:

   ```
   https://localhost:xxxx/
   ```

---

## 📦 Features Implemented

- [x] Data ingestion and normalization
- [x] Blazor Server UI with filters and chart
- [x] National average calculations (preloaded at startup)
- [x] CSV export for provider detail
- [x] Clean, modern responsive UI
- [x] Shared DAL using EF Core

---

## 💬 Assumptions & Notes

- Console app is standalone but requires manual copy of the database
- No external libraries used for ingestion other than CsvHelper
- National averages are computed once and stored in memory for performance
- SQLite decimal ordering handled client-side for compatibility

---
