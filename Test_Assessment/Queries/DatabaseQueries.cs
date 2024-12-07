using Microsoft.Extensions.Logging;
using System.Data.SqlClient;
using Test_Assessment.Interfaces;

namespace Test_Assessment.Queries
{
    public class DatabaseQueries(DatabaseSettings settings, ILogger<DatabaseQueries> logger) : IDatabaseQueries
    {
        private readonly string _connectionString = settings.ConnectionString
                ?? throw new ArgumentNullException(nameof(settings.ConnectionString), "Connection string is not configured.");
        public async Task ExecuteQueriesAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            // Query 1: Highest average tip amount
            var highestTip = await GetHighestAverageTipLocationAsync(connection);
            logger.LogInformation($"Highest average tip: {highestTip.AverageTipAmount} at PULocationID {highestTip.PULocationId}");

            // Query 2: Top 100 longest fares by distance
            var longestByDistance = await GetTop100LongestFaresByDistanceAsync(connection);
            logger.LogInformation("Top 100 longest fares by distance:");
            foreach (var fare in longestByDistance)
            {
                logger.LogInformation($"Id: {fare.Id}, Distance: {fare.TripDistance}");
            }

            // Query 3: Top 100 longest fares by time
            var longestByTime = await GetTop100LongestFaresByTimeAsync(connection);
            logger.LogInformation("Top 100 longest fares by time:");
            foreach (var fare in longestByTime)
            {
                logger.LogInformation($"Id: {fare.Id}, Duration: {fare.Duration}");
            }

            // Query 4: Search trips by Pickup Location ID
            var trips = await SearchTripsByPULocationIdAsync(connection, 239);
            logger.LogInformation("Trips for Pickup Location ID 239:");
            foreach (var trip in trips)
            {
                logger.LogInformation($"Id: {trip.Id}, Pickup: {trip.PickupDatetime}, Dropoff: {trip.DropoffDatetime}, PULocationId: {trip.PULocationId}");
            }
        }

        public async Task<(int PULocationId, decimal AverageTipAmount)> GetHighestAverageTipLocationAsync(SqlConnection connection)
        {
            using var command = new SqlCommand(QueryStrings.GetHighestAverageTipLocation, connection);
            using var reader = await command.ExecuteReaderAsync();

            return reader.Read()
                ? (reader.GetInt32(0), reader.GetDecimal(1))
                : (0, 0);
        }

        public async Task<List<(int Id, double TripDistance)>> GetTop100LongestFaresByDistanceAsync(SqlConnection connection)
        {
            var results = new List<(int Id, double TripDistance)>();
            using var command = new SqlCommand(QueryStrings.GetTop100LongestFaresByDistance, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (reader.Read())
            {
                results.Add((reader.GetInt32(0), reader.GetDouble(1)));
            }

            return results;
        }

        public async Task<List<(int Id, TimeSpan Duration)>> GetTop100LongestFaresByTimeAsync(SqlConnection connection)
        {
            var results = new List<(int Id, TimeSpan Duration)>();
            using var command = new SqlCommand(QueryStrings.GetTop100LongestFaresByTime, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (reader.Read())
            {
                results.Add((reader.GetInt32(0), TimeSpan.FromSeconds(reader.GetInt32(1))));
            }

            return results;
        }

        public async Task<List<(int Id, DateTime PickupDatetime, DateTime DropoffDatetime, int PULocationId)>> SearchTripsByPULocationIdAsync(SqlConnection connection, int pulocationId)
        {
            var results = new List<(int Id, DateTime PickupDatetime, DateTime DropoffDatetime, int PULocationId)>();
            using var command = new SqlCommand(QueryStrings.SearchTripsByPULocationId, connection);
            command.Parameters.AddWithValue("@PULocationId", pulocationId);

            using var reader = await command.ExecuteReaderAsync();
            while (reader.Read())
            {
                results.Add((
                    reader.GetInt32(0),
                    reader.GetDateTime(1),
                    reader.GetDateTime(2),
                    reader.GetInt32(3)
                ));
            }

            return results;
        }
    }
}
