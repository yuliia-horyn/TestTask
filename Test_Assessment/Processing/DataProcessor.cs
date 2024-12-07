using CsvHelper;
using Microsoft.Extensions.Logging;
using System.Globalization;
using Test_Assessment.Helpers;
using Test_Assessment.Interfaces;
using Test_Assessment.Model;

public class DataProcessor(
        IDataParser csvParser,
        IDatabaseInserter databaseInserter,
        IFileWriter fileWriter,
        PathService pathService,
        ILogger<DataProcessor> logger) : IDataProcessor
{

    public async Task ProcessDataAsync()
    {
        var csvFilePath = pathService.CsvFilePath;
        var duplicatesCsvPath = pathService.DuplicatesCsvPath;
        var errorsCsvPath = pathService.ErrorsCsvPath;

        if (string.IsNullOrEmpty(csvFilePath))
        {
            logger.LogError("CSV file path is not provided or is empty.");
            return;
        }

        if (!File.Exists(csvFilePath))
        {
            logger.LogError($"The CSV file does not exist at path: {csvFilePath}");
            return;
        }

        var validTrips = new List<TripModel>();
        var duplicateTrips = new List<TripModel>();
        var errorRecords = new List<(int rowIndex, string rawRecord)>();
        var processedKeys = new HashSet<string>();

        try
        {
            using var fileReader = new StreamReader(csvFilePath);
            using var csvReader = new CsvReader(fileReader, CultureInfo.InvariantCulture);

            csvReader.Read();
            csvReader.ReadHeader();

            int currentRowIndex = 1;
            while (csvReader.Read())
            {
                currentRowIndex++;
                var parseResult = csvParser.ParseCsvRowToTrip(csvReader);

                if (!parseResult.IsValid)
                {
                    errorRecords.Add((currentRowIndex, csvReader.Parser.RawRecord));
                }

                var trip = parseResult.Trip;
                var tripKey = $"{trip.PickupDatetime}-{trip.DropoffDatetime}-{trip.PassengerCount}";

                if (processedKeys.Contains(tripKey))
                    duplicateTrips.Add(trip);
                else
                {
                    processedKeys.Add(tripKey);
                    validTrips.Add(trip);
                }
            }


            logger.LogInformation("Writing duplicate trips to file.");
            await fileWriter.WriteDuplicateTripsAsync(duplicateTrips, duplicatesCsvPath);

            logger.LogInformation("Writing error records to file.");
            await fileWriter.WriteErrorRecordsAsync(errorRecords, errorsCsvPath);

            logger.LogInformation("Inserting valid trips into the database.");
            await databaseInserter.InsertTripsAsync(validTrips);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while processing data.");
        }
    }
}
