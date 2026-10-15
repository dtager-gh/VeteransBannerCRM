
using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using VeteransBannerCRM.Helpers;
using VeteransBannerCRM.Models;

namespace VeteransBannerCRM.Services
{
    public static class ExcelImporter
    {
        public static List<BannerOrder> Import(string filePath)
        {
            System.Text.Encoding.RegisterProvider(
                System.Text.CodePagesEncodingProvider.Instance);

            var orders = new List<BannerOrder>();

            using var stream = File.Open(
                filePath, FileMode.Open, FileAccess.Read);

            using var reader = ExcelReaderFactory.CreateReader(stream);

            var dataSet = reader.AsDataSet(new ExcelDataSetConfiguration
            {
                ConfigureDataTable = _ => new ExcelDataTableConfiguration
                {
                    UseHeaderRow = true
                }
            });

            if (dataSet.Tables.Count == 0)
                return orders;

            DataTable table = dataSet.Tables[0];

            foreach (DataRow row in table.Rows)
            {
                var order = new BannerOrder
                {
                    Season = TextNormalizer.ToTitleCase(
                        GetCellIgnoreCase(row, "Season")),

                    Location = TextNormalizer.ToTitleCase(
                        GetCellIgnoreCase(row, "Location")),

                    LastName = TextNormalizer.ToTitleCase(
                        GetCellIgnoreCase(row, "Last Name")),

                    FirstName = TextNormalizer.ToTitleCase(
                        GetCellIgnoreCase(row, "First Name")),

                    Branch = TextNormalizer.CleanText(
                        GetCellIgnoreCase(row, "Branch")),

                    WarConflict = TextNormalizer.CleanText(
                        GetCellIgnoreCase(row, "War/Conflict")),

                    Medals = TextNormalizer.CleanText(
                        GetCellIgnoreCase(row, "Medals")),

                    BannerType = TextNormalizer.CleanText(
                        GetCellIgnoreCase(row, "Banner Type")),

                    PhotoLayout = TextNormalizer.CleanText(
                        GetCellIgnoreCase(row, "Photo Layout")),

                    PairedWith = TextNormalizer.CleanText(
                        GetCellIgnoreCase(row, "Paired With")),

                    Contact = TextNormalizer.ToTitleCase(
                        GetCellIgnoreCase(row, "Contact")),

                    Address = TextNormalizer.CleanText(
                        GetCellIgnoreCase(row, "Address")),

                    Phone = TextNormalizer.CleanText(
                        GetCellIgnoreCase(row, "Phone")),

                    Email = TextNormalizer.CleanEmail(
                        GetCellIgnoreCase(row, "Email")),

                    LivingDeceased = TextNormalizer.CleanText(
                        GetCellAny(row, "Living/Deceased", "L/D")),

                    KIA = ParseBoolean(
                        GetCellIgnoreCase(row, "KIA")),

                    Misc = TextNormalizer.CleanText(
                        GetCellIgnoreCase(row, "Misc")),

                    NameOnBanner = TextNormalizer.CleanText(
                        GetCellIgnoreCase(row, "Name On Banner")),

                    Notes = TextNormalizer.CleanText(
                        GetCellIgnoreCase(row, "Notes"))
                };

                if (!string.IsNullOrWhiteSpace(order.FirstName) ||
                    !string.IsNullOrWhiteSpace(order.LastName))
                {
                    orders.Add(order);
                }
            }

            return orders;
        }

        private static string GetCellIgnoreCase(
            DataRow row, string columnName)
        {
            foreach (DataColumn column in row.Table.Columns)
            {
                if (string.Equals(
                    column.ColumnName.Trim(),
                    columnName.Trim(),
                    StringComparison.OrdinalIgnoreCase))
                {
                    return row[column]?.ToString() ?? "";
                }
            }

            return "";
        }

        private static string GetCellAny(
            DataRow row, params string[] columnNames)
        {
            foreach (string name in columnNames)
            {
                foreach (DataColumn column in row.Table.Columns)
                {
                    if (string.Equals(
                        column.ColumnName.Trim(),
                        name.Trim(),
                        StringComparison.OrdinalIgnoreCase))
                    {
                        return row[column]?.ToString() ?? "";
                    }
                }
            }

            return "";
        }

        private static bool ParseBoolean(string value)
        {
            string normalized = value.Trim().ToLowerInvariant();

            return normalized == "yes" ||
                   normalized == "true" ||
                   normalized == "1" ||
                   normalized == "y";
        }
    }
}
