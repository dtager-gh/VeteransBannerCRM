using ClosedXML.Excel;
using System.Collections.Generic;
using System.IO;
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

            var worksheet =
                workbook.Worksheets.Add("Banner Orders");

            worksheet.Cell(1, 1).Value = "Season";
            worksheet.Cell(1, 2).Value = "Location";
            worksheet.Cell(1, 3).Value = "Last Name";
            worksheet.Cell(1, 4).Value = "First Name";
            worksheet.Cell(1, 5).Value = "Branch";
            worksheet.Cell(1, 6).Value = "War/Conflict";
            worksheet.Cell(1, 7).Value = "Medals";
            worksheet.Cell(1, 8).Value = "Paired With";
            worksheet.Cell(1, 9).Value = "Contact";
            worksheet.Cell(1, 10).Value = "Phone";
            worksheet.Cell(1, 11).Value = "Email";
            worksheet.Cell(1, 12).Value = "Living/Deceased";
            worksheet.Cell(1, 13).Value = "KIA";
            worksheet.Cell(1, 14).Value = "Name On Banner";
            worksheet.Cell(1, 15).Value = "Notes";

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
                worksheet.Cell(row, 10).Value = order.Phone;
                worksheet.Cell(row, 11).Value = order.Email;
                worksheet.Cell(row, 12).Value = order.LivingDeceased;
                worksheet.Cell(row, 13).Value = order.KIA ? "Yes" : "No";
                worksheet.Cell(row, 14).Value = order.NameOnBanner;
                worksheet.Cell(row, 15).Value = order.Notes;

                row++;
            }

            worksheet.Columns().AdjustToContents();

            workbook.SaveAs(filePath);
        }
    }
}