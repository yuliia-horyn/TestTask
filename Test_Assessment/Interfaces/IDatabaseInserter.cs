using Test_Assessment.Model;

namespace Test_Assessment.Interfaces
{
    public interface IDatabaseInserter
    {
        /// <summary>
        /// Asynchronously inserts a list of trips into the database in batches.
        /// </summary>
        /// <param name="trips">The list of trips to insert.</param>
        /// <param name="batchSize">The size of each batch for insertion. Defaults to 500.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task InsertTripsAsync(List<TripModel> trips, int batchSize = 500, CancellationToken cancellationToken = default);
    }
}
