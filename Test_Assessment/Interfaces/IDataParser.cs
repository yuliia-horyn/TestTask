using Test_Assessment.Model;

namespace Test_Assessment.Interfaces
{
    /// <summary>
    /// Interface for parsing trip data from CSV files.
    /// </summary>
    public interface IDataParser
    {
        /// <summary>
        /// Parses a single row from a CSV file into a <see cref="TripModel"/>.
        /// </summary>
        /// <param name="csvReader">The CSV reader instance.</param>
        /// <returns>A tuple containing a flag indicating validity and the parsed <see cref="TripModel"/>.</returns>
        (bool IsValid, TripModel Trip) ParseCsvRowToTrip(CsvHelper.CsvReader csvReader);
    }
}
