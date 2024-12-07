using Test_Assessment.Model;

namespace Test_Assessment.Interfaces
{
    public interface ICsvParser
    {
        (bool IsValid, TripModel Trip) ParseCsvRowToTrip(CsvHelper.CsvReader csvReader);
    }
}
