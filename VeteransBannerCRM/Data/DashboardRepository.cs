using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using VeteransBannerCRM.Models;
using System;

namespace VeteransBannerCRM.Data
{
    public static class DashboardRepository
    {
        public static List<DashboardCount> GetCountsByLocationAndSeason()
        {
            var counts = new List<DashboardCount>();

            using var connection = new SqliteConnection(Database.ConnectionString);
            connection.Open();

            var command = connection.CreateCommand();

            command.CommandText =
            @"
            SELECT
                Location,
                Season,
                COUNT(*) AS OrderCount
            FROM BannerOrders
            GROUP BY Location, Season
            ORDER BY Location, Season;
            ";

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                counts.Add(new DashboardCount
                {
                    Location = reader.IsDBNull(0) ? "" : reader.GetString(0),
                    Season = reader.IsDBNull(1) ? "" : reader.GetString(1),
                    OrderCount = reader.GetInt32(2)
                });
            }

            return counts;
        }

        public static int GetTotalOrders()
        {
            using var connection =
                new SqliteConnection(Database.ConnectionString);

            connection.Open();

            var command = connection.CreateCommand();

            command.CommandText =
            @"
                SELECT COUNT(*)
                FROM BannerOrders;
                ";

            return Convert.ToInt32(command.ExecuteScalar());
        }

        public static List<DashboardCount> GetCountsBySeason()
        {
            var counts = new List<DashboardCount>();

            using var connection = new SqliteConnection(Database.ConnectionString);
            connection.Open();

            var command = connection.CreateCommand();

            command.CommandText =
            @"
                SELECT
                    Season,
                    COUNT(*) AS OrderCount
                FROM BannerOrders
                GROUP BY Season
                ORDER BY Season;
                ";

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                counts.Add(new DashboardCount
                {
                    Season = reader.GetString(0),
                    OrderCount = reader.GetInt32(1)
                });
            }

            return counts;
        }
    }
}