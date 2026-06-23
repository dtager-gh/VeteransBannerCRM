using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using VeteransBannerCRM.Models;

namespace VeteransBannerCRM.Data
{
    public static class FinancialRepository
    {
        public static void AddDeduction(FinancialTransaction transaction)
        {
            using var connection = new SqliteConnection(Database.ConnectionString);
            connection.Open();

            var command = connection.CreateCommand();

            command.CommandText =
            @"
            INSERT INTO FinancialTransactions
            (
                TransactionDate,
                Season,
                Description,
                Amount
            )
            VALUES
            (
                $TransactionDate,
                $Season,
                $Description,
                $Amount
            );
            ";

            command.Parameters.AddWithValue("$TransactionDate", transaction.TransactionDate);
            command.Parameters.AddWithValue("$Season", transaction.Season);
            command.Parameters.AddWithValue("$Description", transaction.Description);
            command.Parameters.AddWithValue("$Amount", transaction.Amount);

            command.ExecuteNonQuery();
        }

        public static decimal GetDeductionsBySeason(string season)
        {
            using var connection = new SqliteConnection(Database.ConnectionString);
            connection.Open();

            var command = connection.CreateCommand();

            command.CommandText =
            @"
            SELECT COALESCE(SUM(Amount), 0)
            FROM FinancialTransactions
            WHERE Season = $Season;
            ";

            command.Parameters.AddWithValue("$Season", season);

            return Convert.ToDecimal(command.ExecuteScalar());
        }

        public static List<FinancialTransaction> GetAll()
        {
            var transactions = new List<FinancialTransaction>();

            using var connection = new SqliteConnection(Database.ConnectionString);
            connection.Open();

            var command = connection.CreateCommand();

            command.CommandText =
            @"
            SELECT TransactionID, TransactionDate, Season, Description, Amount
            FROM FinancialTransactions
            ORDER BY TransactionDate DESC;
            ";

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                transactions.Add(new FinancialTransaction
                {
                    TransactionID = reader.GetInt32(0),
                    TransactionDate = reader.GetString(1),
                    Season = reader.GetString(2),
                    Description = reader.GetString(3),
                    Amount = reader.GetDecimal(4)
                });
            }

            return transactions;
        }

        public static decimal GetTotalDeductions()
        {
            using var connection =
                new SqliteConnection(Database.ConnectionString);

            connection.Open();

            var command = connection.CreateCommand();

            command.CommandText =
            @"
            SELECT COALESCE(SUM(Amount),0)
            FROM FinancialTransactions;
            ";

            return Convert.ToDecimal(command.ExecuteScalar());
        }
    }
}
