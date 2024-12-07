using Test_Assessment.Model;

namespace Test_Assessment.Interfaces
{
    public interface IFileWriter
    {
        Task WriteErrorRecordsAsync(List<(int rowIndex, string rawRecord)> errorRecords, string errorFilePath);
        Task WriteDuplicateTripsAsync(List<TripModel> duplicateTrips, string duplicatesFilePath);
    }
}
