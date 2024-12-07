using Test_Assessment.Model;

namespace Test_Assessment.Interfaces
{
    /// <summary>
    /// Interface for writing processed trip data to files.
    /// </summary>
    public interface IFileWriter
    {
        /// <summary>
        /// Writes records with errors to the specified error file.
        /// </summary>
        /// <param name="errorRecords">A list of tuples containing row indices and raw records with errors.</param>
        /// <param name="errorFilePath">The path to the error file.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task WriteErrorRecordsAsync(List<(int rowIndex, string rawRecord)> errorRecords, string errorFilePath);

        /// <summary>
        /// Writes duplicate trip data to the specified duplicates file.
        /// </summary>
        /// <param name="duplicateTrips">A list of duplicate trips.</param>
        /// <param name="duplicatesFilePath">The path to the duplicates file.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task WriteDuplicateTripsAsync(List<TripModel> duplicateTrips, string duplicatesFilePath);
    }
}
