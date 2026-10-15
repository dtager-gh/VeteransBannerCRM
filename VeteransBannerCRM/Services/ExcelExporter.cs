
using ClosedXML.Excel;
using System.Collections.Generic;
using VeteransBannerCRM.Models;

namespace VeteransBannerCRM.Services
{
    public static class ExcelExporter
    {
        public static void ExportBannerOrders(
            List<BannerOrder> orders,
            string filePath)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Banner Orders");

            string[] headers =
            {
                "Season",
                "Location",
                "Last Name",
                "First Name",
                "Branch",
                "War/Conflict",
                "Medals",
                "Paired With",
                "Contact",
                "Address",
                "Phone",
                "Email",
                "Living/Deceased",
                "KIA",
                "Banner Type",
                "Photo Layout",
                "Misc",
                "Name On Banner",
                "Notes"
            };

            for (int col = 0; col < headers.Length; col++)
            {
                worksheet.Cell(1, col + 1).Value = headers[col];
            }

            int row = 2;

            foreach (var order in orders)
            {
                worksheet.Cell(row, 1).Value = order.Season;
                worksheet.Cell(row, 2).Value = order.Location;
                worksheet.Cell(row, 3).Value = order.LastName;
                worksheet.Cell(row, 4).Value = order.FirstName;
                worksheet.Cell(row, 5).Value = order.Branch;
                worksheet.Cell(row, 6).Value = order.WarConflict;
                worksheet.Cell(row, 7).Value = order.Medals;
                worksheet.Cell(row, 8).Value = order.PairedWith;
                worksheet.Cell(row, 9).Value = order.Contact;
                worksheet.Cell(row, 10).Value = order.Address;
                worksheet.Cell(row, 11).Value = order.Phone;
                worksheet.Cell(row, 12).Value = order.Email;
                worksheet.Cell(row, 13).Value = order.LivingDeceased;
                worksheet.Cell(row, 14).Value = order.KIA ? "Yes" : "No";
                worksheet.Cell(row, 15).Value = order.BannerType;
                worksheet.Cell(row, 16).Value = order.PhotoLayout;
                worksheet.Cell(row, 17).Value = order.Misc;
                worksheet.Cell(row, 18).Value = order.NameOnBanner;
                worksheet.Cell(row, 19).Value = order.Notes;

                row++;
            }

            // Format header row
            var headerRange = worksheet.Range(1, 1, 1, headers.Length);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.DarkBlue;
            headerRange.Style.Font.FontColor = XLColor.White;

            // Keep headers visible while scrolling
            worksheet.SheetView.FreezeRows(1);

            // Enable Excel filtering
            worksheet.Range(
                1, 1, System.Math.Max(row - 1, 2), headers.Length
            ).SetAutoFilter();

            // Adjust column widths
            worksheet.Columns(1, headers.Length).AdjustToContents();

            workbook.SaveAs(filePath);
        }
    }
}
