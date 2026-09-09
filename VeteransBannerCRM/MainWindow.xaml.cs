using Microsoft.Data.Sqlite;
using Microsoft.Win32;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using VeteransBannerCRM.Data;
using VeteransBannerCRM.Helpers;
using VeteransBannerCRM.Models;
using VeteransBannerCRM.Services;
using System;


namespace VeteransBannerCRM
{
    public partial class MainWindow : Window
    {
        private int selectedOrderId = 0;

        private List<BannerOrder> importedOrders = new List<BannerOrder>();
        private bool isImportPreview = false;
        private int selectedImportIndex = -1;


        public MainWindow()
        {
            InitializeComponent();

            LoadSeasonFilter();
            LoadFinancialSeasons();
            LoadBannerOrders();
            LoadDashboard();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            BannerOrder order = BuildOrderFromForm();

            if (isImportPreview)
            {
                if (selectedImportIndex < 0)
                {
                    MessageBox.Show("Please select an imported row first.");
                    return;
                }

                importedOrders[selectedImportIndex] = BuildOrderFromForm();

                dgVeterans.ItemsSource = null;
                dgVeterans.ItemsSource = importedOrders;

                ClearForm();
                btnSave.Content = "Update Preview Row";

                MessageBox.Show("Preview row updated.");
                return;
            }

            if (selectedOrderId == 0)
            {
                BannerOrderRepository.Add(order);
                MessageBox.Show("Banner Order Saved!");
            }
            else
            {
                order.OrderID = selectedOrderId;
                BannerOrderRepository.Update(order);
                MessageBox.Show("Banner Order Updated!");
            }

            ClearForm();
            LoadSeasonFilter();
            LoadFinancialSeasons();
            LoadBannerOrders();
            LoadDashboard();
        }

        private BannerOrder BuildOrderFromForm()
        {
            string branch = ((ComboBoxItem)cmbBranch.SelectedItem)?.Content?.ToString() ?? "";
            string location = ((ComboBoxItem)cmbLocation.SelectedItem)?.Content?.ToString() ?? "";
            string bannerType = ((ComboBoxItem)cmbBannerType.SelectedItem)?.Content?.ToString() ?? "";
            string photoLayout = ((ComboBoxItem)cmbPhotoLayout.SelectedItem)?.Content?.ToString() ?? "";

            string livingDeceased = "";

            if (rbLiving.IsChecked == true)
            {
                livingDeceased = "Living";
            }
            else if (rbDeceased.IsChecked == true)
            {
                livingDeceased = "Deceased";
            }

            return new BannerOrder
            {
                Season = GetSeason(),
                Location = location,
                FirstName = TextNormalizer.ToTitleCase(txtFirstName.Text),
                LastName = TextNormalizer.ToTitleCase(txtLastName.Text),
                Branch = branch,
                WarConflict = TextNormalizer.CleanText(txtWarConflict.Text),
                Medals = TextNormalizer.CleanText(txtMedals.Text),
                BannerType = bannerType,
                PhotoLayout = photoLayout,
                PairedWith = TextNormalizer.CleanText(txtPairedWith.Text),
                Contact = TextNormalizer.ToTitleCase(txtContact.Text),
                Address = TextNormalizer.CleanText(txtAddress.Text),
                Phone = TextNormalizer.CleanText(txtPhone.Text),
                Email = TextNormalizer.CleanEmail(txtEmail.Text),
                LivingDeceased = livingDeceased,
                KIA = chkKIA.IsChecked == true,
                Misc = TextNormalizer.CleanText(txtMisc.Text),
                NameOnBanner = TextNormalizer.CleanText(txtNameOnBanner.Text),
                Notes = TextNormalizer.CleanText(txtNotes.Text)
            };
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (selectedOrderId == 0)
            {
                MessageBox.Show("Please select an order first.");
                return;
            }

            var result = MessageBox.Show(
                "Are you sure you want to delete this order?",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
            {
                return;
            }

            BannerOrderRepository.Delete(selectedOrderId);

            MessageBox.Show("Order Deleted.");

            ClearForm();
            LoadSeasonFilter();
            LoadFinancialSeasons();
            LoadBannerOrders();
            LoadDashboard();
        }

        private void btnNewOrder_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();

            dgVeterans.SelectedItem = null;

            isImportPreview = false;
            selectedImportIndex = -1;

            LoadBannerOrders(txtSearch.Text);
        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            if (dgVeterans.ItemsSource is not IEnumerable<BannerOrder> orders)
            {
                MessageBox.Show("No orders to export.");
                return;
            }

            var orderList = orders.ToList();

            if (orderList.Count == 0)
            {
                MessageBox.Show("No orders to export.");
                return;
            }

            string seasonFilter =
                ((ComboBoxItem)cmbSeasonFilter.SelectedItem)?
                .Content?.ToString() ?? "All";

            string fileName =
                seasonFilter == "All"
                    ? "AllBannerOrders.xlsx"
                    : $"{seasonFilter.Replace(" ", "")}BannerOrders.xlsx";

            string exportFolder =
                Path.Combine(Database.AppFolder, "exports");

            Directory.CreateDirectory(exportFolder);

            string filePath =
                Path.Combine(exportFolder, fileName);

            ExcelExporter.ExportBannerOrders(orderList, filePath);

            MessageBox.Show($"Export created:\n{filePath}");
        }

        private void btnImport_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Title = "Select Excel File",
                Filter = "Excel Files (*.xlsx;*.xls)|*.xlsx;*.xls"
            };

            if (dialog.ShowDialog() != true)
            {
                return;
            }

            importedOrders = ExcelImporter.Import(dialog.FileName);

            isImportPreview = true;
            selectedImportIndex = -1;
            selectedOrderId = 0;

            btnSave.Content = "Update Preview Row";

            MessageBox.Show($"Found {importedOrders.Count} orders in the Excel file.");

            dgVeterans.ItemsSource = null;
            dgVeterans.ItemsSource = importedOrders;
        }

        private void btnSaveImport_Click(object sender, RoutedEventArgs e)
        {
            if (importedOrders.Count == 0)
            {
                MessageBox.Show("No imported records to save.");
                return;
            }

            var result = MessageBox.Show(
                $"Save {importedOrders.Count} imported records to the database?",
                "Confirm Import",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
            {
                return;
            }

            int importedCount = 0;
            int skippedCount = 0;

            foreach (var order in importedOrders)
            {
                if (BannerOrderRepository.Exists(order))
                {
                    skippedCount++;
                    continue;
                }

                BannerOrderRepository.Add(order);
                importedCount++;
            }

            MessageBox.Show(
                $"Import complete.\n\nImported: {importedCount}\nSkipped duplicates: {skippedCount}");

            importedOrders.Clear();
            isImportPreview = false;
            selectedImportIndex = -1;

            ClearForm();
            LoadSeasonFilter();
            LoadFinancialSeasons();
            LoadBannerOrders();
            LoadDashboard();
        }


        private void ClearForm()
        {
            selectedOrderId = 0;
            btnSave.Content = "Save Banner Order";

            cmbSeasonName.SelectedIndex = -1;
            cmbSeasonYear.SelectedIndex = -1;
            cmbLocation.SelectedIndex = -1;

            txtFirstName.Clear();
            txtLastName.Clear();

            cmbBranch.SelectedIndex = -1;

            txtWarConflict.Clear();
            txtMedals.Clear();

            rbLiving.IsChecked = false;
            rbDeceased.IsChecked = false;
            chkKIA.IsChecked = false;

            txtContact.Clear();
            txtPhone.Clear();
            txtEmail.Clear();
            txtAddress.Clear();

            txtPairedWith.Clear();
            txtMisc.Clear();
            txtNameOnBanner.Clear();
            txtNotes.Clear();

            cmbBannerType.SelectedIndex = -1;
            cmbPhotoLayout.SelectedIndex = -1;
        }

        private void LoadBannerOrders(string searchText = "")
        {
            string seasonFilter =
                ((ComboBoxItem)cmbSeasonFilter.SelectedItem)?
                .Content?.ToString() ?? "All";

            dgVeterans.ItemsSource =
                BannerOrderRepository.Search(searchText, seasonFilter);
        }

        private void LoadSeasonFilter()
        {
            cmbSeasonFilter.Items.Clear();

            cmbSeasonFilter.Items.Add(
                new ComboBoxItem { Content = "All" });

            using var connection =
                new SqliteConnection(Database.ConnectionString);

            connection.Open();

            var command = connection.CreateCommand();

            command.CommandText =
            @"
                SELECT DISTINCT Season
                FROM BannerOrders
                WHERE Season IS NOT NULL
                ORDER BY Season;
                ";

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                cmbSeasonFilter.Items.Add(
                    new ComboBoxItem
                    {
                        Content = reader.GetString(0)
                    });
            }

            cmbSeasonFilter.SelectedIndex = 0;
        }

        private void dgVeterans_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgVeterans.SelectedItem is not BannerOrder order)
            {
                return;
            }

            if (isImportPreview)
            {
                selectedImportIndex = importedOrders.IndexOf(order);
                LoadOrderIntoForm(order);
                btnSave.Content = "Update Preview Row";
                return;
            }

            selectedOrderId = order.OrderID;
            btnSave.Content = "Save Changes";

            LoadOrderIntoForm(order);
        }

        private void LoadOrderIntoForm(BannerOrder order)
        {
            SetSeasonComboBoxes(order.Season);
            txtFirstName.Text = order.FirstName;
            txtLastName.Text = order.LastName;
            txtWarConflict.Text = order.WarConflict;
            txtMedals.Text = order.Medals;

            txtContact.Text = order.Contact;
            txtPhone.Text = order.Phone;
            txtEmail.Text = order.Email;
            txtAddress.Text = order.Address;

            txtPairedWith.Text = order.PairedWith;
            txtMisc.Text = order.Misc;
            txtNameOnBanner.Text = order.NameOnBanner;
            txtNotes.Text = order.Notes;

            SetComboBoxValue(cmbLocation, order.Location);
            SetComboBoxValue(cmbBranch, order.Branch);
            SetComboBoxValue(cmbBannerType, order.BannerType);
            SetComboBoxValue(cmbPhotoLayout, order.PhotoLayout);

            rbLiving.IsChecked = order.LivingDeceased == "Living";
            rbDeceased.IsChecked = order.LivingDeceased == "Deceased";
            chkKIA.IsChecked = order.KIA;
        }

        private void SetComboBoxValue(ComboBox comboBox, string value)
        {
            foreach (ComboBoxItem item in comboBox.Items)
            {
                if (item.Content?.ToString() == value)
                {
                    comboBox.SelectedItem = item;
                    return;
                }
            }

            comboBox.SelectedIndex = -1;
        }

        private void cmbSeasonFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadBannerOrders(txtSearch.Text);
        }


        private string GetSeason()
        {
            string seasonName = cmbSeasonName.Text;
            string seasonYear = cmbSeasonYear.Text;

            return TextNormalizer.CleanText($"{seasonName} {seasonYear}");
        }


        private void SetSeasonComboBoxes(string season)
        {
            string cleanSeason = TextNormalizer.CleanText(season);

            cmbSeasonName.SelectedIndex = -1;
            cmbSeasonYear.SelectedIndex = -1;

            if (string.IsNullOrWhiteSpace(cleanSeason))
            {
                cmbSeasonName.Text = "";
                cmbSeasonYear.Text = "";
                return;
            }

            string[] parts = cleanSeason.Split(' ');

            string year = parts.LastOrDefault(p => int.TryParse(p, out _)) ?? "";

            string seasonName = cleanSeason.Replace(year, "").Trim();

            cmbSeasonName.Text = seasonName;
            cmbSeasonYear.Text = year;
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoadBannerOrders(txtSearch.Text);
        }

        private void LoadDashboard()
        {
            var seasonTotals = DashboardRepository.GetCountsBySeason();
            var townTotals = DashboardRepository.GetCountsByLocationAndSeason();

            var sb = new StringBuilder();
            var deductionsList = FinancialRepository.GetAll();
            int totalOrders = DashboardRepository.GetTotalOrders();

            decimal revenue = DashboardRepository.GetTotalRevenue();

            decimal deductions = FinancialRepository.GetTotalDeductions();

            decimal balance = revenue - deductions;

            txtBalance.Text =
                $"Revenue: {revenue:C}    " +
                $"Deductions: {deductions:C}    " +
                $"Balance: {balance:C}";

            sb.AppendLine("VETERANS BANNER DASHBOARD");
            sb.AppendLine($"Last Updated: {DateTime.Now}");
            sb.AppendLine("");
            sb.AppendLine($"TOTAL ORDERS: {DashboardRepository.GetTotalOrders()}");
            sb.AppendLine("");

            sb.AppendLine("COUNTY TOTALS");
            sb.AppendLine("--------------------------------");

            foreach (var season in seasonTotals)
            {
                sb.AppendLine($"{season.Season,-25}{season.OrderCount}");
            }

            sb.AppendLine("--------------------------------");
            sb.AppendLine($"COUNTY TOTAL:{"",13}{seasonTotals.Sum(x => x.OrderCount)}");
            sb.AppendLine("");

            foreach (var locationGroup in townTotals.GroupBy(x => x.Location))
            {
                sb.AppendLine(locationGroup.Key.ToUpper());
                sb.AppendLine("--------------------------------");

                foreach (var item in locationGroup)
                {
                    sb.AppendLine($"{item.Season,-25}{item.OrderCount}");
                }

                sb.AppendLine("--------------------------------");
                sb.AppendLine($"TOTAL:{"",20}{locationGroup.Sum(x => x.OrderCount)}");
                sb.AppendLine("");
            }

            sb.AppendLine("DEDUCTION HISTORY");
            sb.AppendLine("--------------------------------");

            foreach (var deduction in deductionsList)
            {
                sb.AppendLine(
                    $"{deduction.TransactionDate,-15}" +
                    $"{deduction.Description,-30}" +
                    $"{deduction.Amount:C}");
            }

            txtDashboard.Text = sb.ToString();
        }

        private void btnBackup_Click(object sender, RoutedEventArgs e)
        {
            string backupFolder = Path.Combine(Database.AppFolder, "backups");

            Directory.CreateDirectory(backupFolder);

            string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HHmmss");

            string backupFile = Path.Combine(
                backupFolder,
                $"VeteransBannerCRM_{timestamp}.db");

            File.Copy(Database.DbPath, backupFile, true);

            MessageBox.Show($"Database backup created:\n{backupFile}");
        }

        private void btnAddDeduction_Click(object sender, RoutedEventArgs e)
        {
            if (!decimal.TryParse(txtDeductionAmount.Text, out decimal amount))
            {
                MessageBox.Show("Enter a valid amount.");
                return;
            }

            string season =
                ((ComboBoxItem)cmbFinancialSeason.SelectedItem)?
                .Content?.ToString() ?? "";

            FinancialRepository.AddDeduction(
                new FinancialTransaction
                {
                    TransactionDate = DateTime.Now.ToString("yyyy-MM-dd"),
                    Season = season,
                    Description = txtDeductionDescription.Text,
                    Amount = amount
                });

            MessageBox.Show("Deduction added.");

            txtDeductionDescription.Clear();
            txtDeductionAmount.Clear();
            LoadDashboard();

        }

        private void LoadFinancialSeasons()
        {
            cmbFinancialSeason.Items.Clear();

            foreach (ComboBoxItem item in cmbSeasonFilter.Items)
            {
                string season = item.Content?.ToString() ?? "";

                if (season != "All")
                {
                    cmbFinancialSeason.Items.Add(
                        new ComboBoxItem
                        {
                            Content = season
                        });
                }
            }

            if (cmbFinancialSeason.Items.Count > 0)
            {
                cmbFinancialSeason.SelectedIndex = 0;
            }
        }
    }
}