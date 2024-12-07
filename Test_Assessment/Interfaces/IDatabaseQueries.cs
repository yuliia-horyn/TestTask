namespace Test_Assessment.Interfaces
{
    public interface IDatabaseQueries
    {
        Task<(int PULocationId, decimal AverageTipAmount)> GetHighestAverageTipLocationAsync();
        Task<List<(int Id, double TripDistance)>> GetTop100LongestFaresByDistanceAsync();
        Task<List<(int Id, TimeSpan Duration)>> GetTop100LongestFaresByTimeAsync();
    }
}
