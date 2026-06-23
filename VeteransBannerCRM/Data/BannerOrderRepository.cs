using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using VeteransBannerCRM.Models;
using VeteransBannerCRM.Data;

namespace VeteransBannerCRM.Data
{
    public static class BannerOrderRepository
    {
        public static List<BannerOrder> Search(string searchText, string seasonFilter)
        {
            var orders = new List<BannerOrder>();

            using var connection = new SqliteConnection(Database.ConnectionString);
            connection.Open();

            var command = connection.CreateCommand();

            command.CommandText =
            @"
            SELECT
                OrderID, Season, Location, FirstName, LastName, Branch,
                WarConflict, Medals, BannerType, PhotoLayout, PairedWith,
                Contact, Address, Phone, Email, LivingDeceased, KIA,
                Misc, NameOnBanner, Notes
            FROM BannerOrders
            WHERE
            (
                FirstName LIKE $Search
                OR LastName LIKE $Search
                OR Branch LIKE $Search
                OR WarConflict LIKE $Search
                OR Season LIKE $Search
                OR Location LIKE $Search
                OR Contact LIKE $Search
                OR Phone LIKE $Search
                OR Email LIKE $Search
                OR Misc LIKE $Search
                OR NameOnBanner LIKE $Search
            )
            AND
            (
                $SeasonFilter = 'All'
                OR Season = $SeasonFilter
            )
            ORDER BY LastName;
            ";

            command.Parameters.AddWithValue("$Search", $"%{searchText}%");
            command.Parameters.AddWithValue("$SeasonFilter", seasonFilter);

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                orders.Add(new BannerOrder
                {
                    OrderID = reader.GetInt32(0),
                    Season = reader.IsDBNull(1) ? "" : reader.GetString(1),
                    Location = reader.IsDBNull(2) ? "" : reader.GetString(2),
                    FirstName = reader.IsDBNull(3) ? "" : reader.GetString(3),
                    LastName = reader.IsDBNull(4) ? "" : reader.GetString(4),
                    Branch = reader.IsDBNull(5) ? "" : reader.GetString(5),
                    WarConflict = reader.IsDBNull(6) ? "" : reader.GetString(6),
                    Medals = reader.IsDBNull(7) ? "" : reader.GetString(7),
                    BannerType = reader.IsDBNull(8) ? "" : reader.GetString(8),
                    PhotoLayout = reader.IsDBNull(9) ? "" : reader.GetString(9),
                    PairedWith = reader.IsDBNull(10) ? "" : reader.GetString(10),
                    Contact = reader.IsDBNull(11) ? "" : reader.GetString(11),
                    Address = reader.IsDBNull(12) ? "" : reader.GetString(12),
                    Phone = reader.IsDBNull(13) ? "" : reader.GetString(13),
                    Email = reader.IsDBNull(14) ? "" : reader.GetString(14),
                    LivingDeceased = reader.IsDBNull(15) ? "" : reader.GetString(15),
                    KIA = !reader.IsDBNull(16) && reader.GetInt32(16) == 1,
                    Misc = reader.IsDBNull(17) ? "" : reader.GetString(17),
                    NameOnBanner = reader.IsDBNull(18) ? "" : reader.GetString(18),
                    Notes = reader.IsDBNull(19) ? "" : reader.GetString(19)
                });
            }

            return orders;
        }

        public static void Add(BannerOrder order)
        {
            Save(order, false);
        }

        public static void Update(BannerOrder order)
        {
            Save(order, true);
        }

        public static void Delete(int orderId)
        {
            using var connection = new SqliteConnection(Database.ConnectionString);
            connection.Open();

            var command = connection.CreateCommand();

            command.CommandText =
            @"
                DELETE FROM BannerOrders
                WHERE OrderID = $OrderID;
                ";

            command.Parameters.AddWithValue("$OrderID", orderId);

            command.ExecuteNonQuery();
        }

        private static void Save(BannerOrder order, bool isUpdate)
        {
            using var connection = new SqliteConnection(Database.ConnectionString);
            connection.Open();

            var command = connection.CreateCommand();

            if (isUpdate)
            {
                command.CommandText =
                @"
                    UPDATE BannerOrders
                    SET
                        Season = $Season,
                        Location = $Location,
                        FirstName = $FirstName,
                        LastName = $LastName,
                        Branch = $Branch,
                        WarConflict = $WarConflict,
                        Medals = $Medals,
                        BannerType = $BannerType,
                        PhotoLayout = $PhotoLayout,
                        PairedWith = $PairedWith,
                        Contact = $Contact,
                        Address = $Address,
                        Phone = $Phone,
                        Email = $Email,
                        LivingDeceased = $LivingDeceased,
                        KIA = $KIA,
                        Misc = $Misc,
                        NameOnBanner = $NameOnBanner,
                        Notes = $Notes
                    WHERE OrderID = $OrderID;
                    ";

                command.Parameters.AddWithValue("$OrderID", order.OrderID);
            }
            else
            {
                command.CommandText =
                @"
                    INSERT INTO BannerOrders
                    (
                        Season,
                        Location,
                        FirstName,
                        LastName,
                        Branch,
                        WarConflict,
                        Medals,
                        BannerType,
                        PhotoLayout,
                        PairedWith,
                        Contact,
                        Address,
                        Phone,
                        Email,
                        LivingDeceased,
                        KIA,
                        Misc,
                        NameOnBanner,
                        Notes
                    )
                    VALUES
                    (
                        $Season,
                        $Location,
                        $FirstName,
                        $LastName,
                        $Branch,
                        $WarConflict,
                        $Medals,
                        $BannerType,
                        $PhotoLayout,
                        $PairedWith,
                        $Contact,
                        $Address,
                        $Phone,
                        $Email,
                        $LivingDeceased,
                        $KIA,
                        $Misc,
                        $NameOnBanner,
                        $Notes
                    );
                    ";
            }

            AddParameters(command, order);
            command.ExecuteNonQuery();
        }

        private static void AddParameters(SqliteCommand command, BannerOrder order)
        {
            command.Parameters.AddWithValue("$Season", order.Season);
            command.Parameters.AddWithValue("$Location", order.Location);
            command.Parameters.AddWithValue("$FirstName", order.FirstName);
            command.Parameters.AddWithValue("$LastName", order.LastName);
            command.Parameters.AddWithValue("$Branch", order.Branch);
            command.Parameters.AddWithValue("$WarConflict", order.WarConflict);
            command.Parameters.AddWithValue("$Medals", order.Medals);
            command.Parameters.AddWithValue("$BannerType", order.BannerType);
            command.Parameters.AddWithValue("$PhotoLayout", order.PhotoLayout);
            command.Parameters.AddWithValue("$PairedWith", order.PairedWith);
            command.Parameters.AddWithValue("$Contact", order.Contact);
            command.Parameters.AddWithValue("$Address", order.Address);
            command.Parameters.AddWithValue("$Phone", order.Phone);
            command.Parameters.AddWithValue("$Email", order.Email);
            command.Parameters.AddWithValue("$LivingDeceased", order.LivingDeceased);
            command.Parameters.AddWithValue("$KIA", order.KIA ? 1 : 0);
            command.Parameters.AddWithValue("$Misc", order.Misc);
            command.Parameters.AddWithValue("$NameOnBanner", order.NameOnBanner);
            command.Parameters.AddWithValue("$Notes", order.Notes);
        }

        public static bool Exists(BannerOrder order)
        {
            using var connection = new SqliteConnection(Database.ConnectionString);
            connection.Open();

            var command = connection.CreateCommand();

            command.CommandText =
            @"
            SELECT COUNT(*)
            FROM BannerOrders
            WHERE Season = $Season
              AND Location = $Location
              AND FirstName = $FirstName
              AND LastName = $LastName;
            ";

            command.Parameters.AddWithValue("$Season", order.Season);
            command.Parameters.AddWithValue("$Location", order.Location);
            command.Parameters.AddWithValue("$FirstName", order.FirstName);
            command.Parameters.AddWithValue("$LastName", order.LastName);

            long count = (long)command.ExecuteScalar();

            return count > 0;
        }
    }
}