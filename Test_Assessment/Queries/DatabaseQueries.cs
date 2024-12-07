using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test_Assessment.Queries
{
    public class DatabaseQueries
    {
        private readonly string _connectionString;

        public DatabaseQueries(string connectionString)
        {
            _connectionString = connectionString;
        }

        public (int PULocationId, decimal AverageTipAmount) GetHighestAverageTipLocation(SqlConnection connection)
        {
            string query = @"
                SELECT TOP 1 PULocationID, AVG(tip_amount) AS AverageTipAmount
                FROM [ETL].[dbo].[TripModel]
                GROUP BY PULocationID
                ORDER BY AverageTipAmount DESC;
            ";

            using (var command = new SqlCommand(query, connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return (reader.GetInt32(0), reader.GetDecimal(1));
                    }
                }
            }

            return (0, 0); 
        }

        public List<(int Id, double TripDistance)> GetTop100LongestFaresByDistance(SqlConnection connection)
        {
            string query = @"
                SELECT TOP 100 Id, trip_distance
                FROM [ETL].[dbo].[TripModel]
                ORDER BY trip_distance DESC;
            ";

            var results = new List<(int Id, double TripDistance)>();

            using (var command = new SqlCommand(query, connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        results.Add((reader.GetInt32(0), reader.GetDouble(1)));
                    }
                }
            }

            return results;
        }

        public List<(int Id, TimeSpan Duration)> GetTop100LongestFaresByTime(SqlConnection connection)
        {
            string query = @"
                SELECT TOP 100 Id, DATEDIFF(SECOND, tpep_pickup_datetime, tpep_dropoff_datetime) AS DurationInSeconds
                FROM [ETL].[dbo].[TripModel]
                ORDER BY DurationInSeconds DESC;
            ";

            var results = new List<(int Id, TimeSpan Duration)>();

            using (var command = new SqlCommand(query, connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var duration = TimeSpan.FromSeconds(reader.GetInt32(1));
                        results.Add((reader.GetInt32(0), duration));
                    }
                }
            }

            return results;
        }

        public List<(int Id, DateTime PickupDatetime, DateTime DropoffDatetime, int PULocationId)> SearchTripsByPULocationId(SqlConnection connection, int pulocationId)
        {
            string query = @"
                SELECT Id, tpep_pickup_datetime, tpep_dropoff_datetime, PULocationID
                FROM [ETL].[dbo].[TripModel]
                WHERE PULocationID = @PULocationId;
            ";

            var results = new List<(int Id, DateTime PickupDatetime, DateTime DropoffDatetime, int PULocationId)>();

            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@PULocationId", pulocationId);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        results.Add((
                            reader.GetInt32(0),
                            reader.GetDateTime(1),
                            reader.GetDateTime(2),
                            reader.GetInt32(3)
                        ));
                    }
                }
            }

            return results;
        }
    }
}
