# Veterans Banner CRM

A C# WPF desktop application for managing Veterans Banner program orders, imports, exports, dashboard reporting, and financial tracking.

## Features

- Add, edit, delete, and search banner orders
- Season and location filtering
- Excel import for existing banner spreadsheets
- Duplicate detection during import
- Excel export for vendor/order reporting
- Dashboard totals by season and town
- County-wide financial balance tracking
- Deduction history
- SQLite database storage
- Portable external-drive deployment

## Technologies Used

- C#
- WPF
- SQLite
- ClosedXML
- ExcelDataReader

## Project Structure

```text
Data/
  Database.cs
  BannerOrderRepository.cs
  DashboardRepository.cs
  FinancialRepository.cs

Helpers/
  TextNormalizer.cs

Models/
  BannerOrder.cs
  DashboardCount.cs
  FinancialTransaction.cs

Services/
  ExcelImporter.cs
  ExcelExporter.cs