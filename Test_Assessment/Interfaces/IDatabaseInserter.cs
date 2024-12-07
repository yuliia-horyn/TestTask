using Test_Assessment.Model;

namespace Test_Assessment.Interfaces
{
    public interface IDatabaseInserter
    {
        Task InsertTripsAsync(List<TripModel> trips, int batchSize = 500);
    }
}
