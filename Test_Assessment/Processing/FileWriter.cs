using CsvHelper;
using System.Globalization;
using Test_Assessment.Interfaces;
using Test_Assessment.Model;

public class FileWriter : IFileWriter
{
    public async Task WriteErrorRecordsAsync(List<(int rowIndex, string rawRecord)> errorRecords, string errorFilePath)
    {
        using var fileWriter = new StreamWriter(errorFilePath);
        foreach (var error in errorRecords)
        {
            await fileWriter.WriteAsync($"Row {error.rowIndex}: {error.rawRecord}");
        }
    }

    public async Task WriteDuplicateTripsAsync(List<TripModel> duplicateTrips, string duplicatesFilePath)
    {
        using var fileWriter = new StreamWriter(duplicatesFilePath);
        using var csvWriter = new CsvWriter(fileWriter, CultureInfo.InvariantCulture);
        await Task.Run(() => csvWriter.WriteRecords(duplicateTrips));
    }
}
