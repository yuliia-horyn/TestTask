using System.Data.SqlClient;

namespace Test_Assessment.Interfaces
{
    /// <summary>
    /// Interface for executing database queries related to trip data.
    /// </summary>
    public interface IDatabaseQueries
    {
        /// <summary>
        /// Executes all predefined queries and logs the results.
        /// </summary>
        Task ExecuteQueriesAsync();

        /// <summary>
        /// Retrieves the location with the highest average tip amount.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <returns>A tuple containing the Pickup Location ID and the Average Tip Amount.</returns>
        Task<(int PULocationId, decimal AverageTipAmount)> GetHighestAverageTipLocationAsync(SqlConnection connection);

        /// <summary>
        /// Retrieves the top 100 longest fares by distance.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <returns>A list of tuples containing the fare ID and the trip distance.</returns>
        Task<List<(int Id, double TripDistance)>> GetTop100LongestFaresByDistanceAsync(SqlConnection connection);

        /// <summary>
        /// Retrieves the top 100 longest fares by trip duration.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <returns>A list of tuples containing the fare ID and the trip duration.</returns>
        Task<List<(int Id, TimeSpan Duration)>> GetTop100LongestFaresByTimeAsync(SqlConnection connection);

        /// <summary>
        /// Searches trips by Pickup Location ID.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <param name="pulocationId">Pickup Location ID.</param>
        /// <returns>A list of tuples containing trip details.</returns>
        Task<List<(int Id, DateTime PickupDatetime, DateTime DropoffDatetime, int PULocationId)>> SearchTripsByPULocationIdAsync(SqlConnection connection, int pulocationId);
    }
}
