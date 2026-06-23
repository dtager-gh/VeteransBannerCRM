using Microsoft.Data.Sqlite;
using System.IO;
using System;

namespace VeteransBannerCRM.Data
{
    public static class Database
    {
        public static string AppFolder = AppDomain.CurrentDomain.BaseDirectory;
        public static string DbPath = Path.Combine(AppFolder, "data", "VeteransBannerCRM.db");

        public static string ConnectionString => $"Data Source={DbPath}";

        public static void Initialize()
        {
            Directory.CreateDirectory(Path.Combine(AppFolder, "data"));
            Directory.CreateDirectory(Path.Combine(AppFolder, "photos"));
            Directory.CreateDirectory(Path.Combine(AppFolder, "order_forms"));
            Directory.CreateDirectory(Path.Combine(AppFolder, "invoices"));
            Directory.CreateDirectory(Path.Combine(AppFolder, "exports"));
            Directory.CreateDirectory(Path.Combine(AppFolder, "backups"));

            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText =
                @"
                CREATE TABLE IF NOT EXISTS BannerOrders (
                    OrderID INTEGER PRIMARY KEY AUTOINCREMENT,

                    Season TEXT,
                    Location TEXT,

                    FirstName TEXT NOT NULL,
                    LastName TEXT NOT NULL,

                    Branch TEXT,
                    WarConflict TEXT,
                    Medals TEXT,

                    PairedWith TEXT,

                    Contact TEXT,
                    Address TEXT,
                    Phone TEXT,
                    Email TEXT,

                    LivingDeceased TEXT,
                    KIA INTEGER DEFAULT 0,
                    BannerType TEXT,
                    PhotoLayout TEXT,
                    Misc TEXT,
                    NameOnBanner TEXT,

                    Notes TEXT
                );
                CREATE TABLE IF NOT EXISTS FinancialTransactions (
                    TransactionID INTEGER PRIMARY KEY AUTOINCREMENT,
                    TransactionDate TEXT NOT NULL,
                    Season TEXT NOT NULL,
                    Description TEXT NOT NULL,
                    Amount REAL NOT NULL
                );
                ";
            command.ExecuteNonQuery();
        }
    }
}