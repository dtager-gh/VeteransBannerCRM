using ExcelDataReader;
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

            using var stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
            using var reader = ExcelReaderFactory.CreateReader(stream);

            var dataSet = reader.AsDataSet(new ExcelDataSetConfiguration
            {
                ConfigureDataTable = _ => new ExcelDataTableConfiguration
                {
                    UseHeaderRow = true
                }
            });

            DataTable table = dataSet.Tables[0];

            foreach (DataRow row in table.Rows)
            {
                var order = new BannerOrder
                {
                    Season = TextNormalizer.ToTitleCase(GetCellIgnoreCase(row, "Season")),
                    Location = TextNormalizer.ToTitleCase(GetCellIgnoreCase(row, "Location")),
                    LastName = TextNormalizer.ToTitleCase(GetCellIgnoreCase(row, "Last Name")),
                    FirstName = TextNormalizer.ToTitleCase(GetCellIgnoreCase(row, "First Name")),
                    Branch = TextNormalizer.CleanText(GetCellIgnoreCase(row, "Branch")),
                    WarConflict = TextNormalizer.CleanText(GetCellIgnoreCase(row, "War/Conflict")),
                    Medals = TextNormalizer.CleanText(GetCellIgnoreCase(row, "Medals")),
                    PairedWith = TextNormalizer.CleanText(GetCellIgnoreCase(row, "PAIRED WITH")),
                    Contact = TextNormalizer.ToTitleCase(GetCellIgnoreCase(row, "Contact")),
                    Address = TextNormalizer.CleanText(GetCellIgnoreCase(row, "Address")),
                    Phone = TextNormalizer.CleanText(GetCellIgnoreCase(row, "Phone")),
                    Email = TextNormalizer.CleanEmail(GetCellIgnoreCase(row, "Email")),
                    LivingDeceased = TextNormalizer.CleanText(GetCellIgnoreCase(row, "L/D")),
                    Misc = TextNormalizer.CleanText(GetCellIgnoreCase(row, "Misc")),
                    NameOnBanner = TextNormalizer.CleanText(GetCellIgnoreCase(row, "Name On Banner")),
                    Notes = ""
                };

                if (!string.IsNullOrWhiteSpace(order.FirstName) ||
                    !string.IsNullOrWhiteSpace(order.LastName))
                {
                    orders.Add(order);
                }
            }

            return orders;
        }

        private static string GetCell(DataRow row, string columnName)
        {
            if (!row.Table.Columns.Contains(columnName))
            {
                return "";
            }

            return row[columnName]?.ToString() ?? "";
        }

        private static string GetCellIgnoreCase(DataRow row, string columnName)
        {
            foreach (DataColumn column in row.Table.Columns)
            {
                if (column.ColumnName.Trim().ToUpper() ==
                    columnName.Trim().ToUpper())
                {
                    return row[column]?.ToString() ?? "";
                }
            }

            return "";
        }
    }


}