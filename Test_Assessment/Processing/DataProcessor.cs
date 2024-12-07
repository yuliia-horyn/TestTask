using CsvHelper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.IO;
using Test_Assessment.Interfaces;
using Test_Assessment.Model;

public class DataProcessor : IDataProcessor
{
    private readonly ICsvParser _csvParser;
    private readonly IDatabaseInserter _databaseInserter;
    private readonly IFileWriter _fileWriter;
    private readonly IConfiguration _configuration;
    private readonly ILogger<DataProcessor> _logger;

    public DataProcessor(
        ICsvParser csvParser,
        IDatabaseInserter databaseInserter,
        IFileWriter fileWriter,
        IConfiguration configuration,
        ILogger<DataProcessor> logger)
    {
        _csvParser = csvParser;
        _databaseInserter = databaseInserter;
        _fileWriter = fileWriter;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task ProcessDataAsync()
    {
        string path = Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..");
        string absolutePath = Path.GetFullPath(path);
        string csvFilePath = Path.Combine(absolutePath, "Data", "sample-cab-data.csv");
        string duplicatesCsvPath = Path.Combine(absolutePath, "Data", "duplicates.csv");
        string errorsCsvPath = Path.Combine(absolutePath, "Data", "errors.csv");

        if (string.IsNullOrEmpty(csvFilePath))
        {
            _logger.LogError("CSV file path is not provided or is empty.");
            return;
        }

        if (!File.Exists(csvFilePath))
        {
            _logger.LogError($"The CSV file does not exist at path: {csvFilePath}");
            return;
        }

        var validTrips = new List<TripModel>();
        var duplicateTrips = new List<TripModel>();
        var errorRecords = new List<(int rowIndex, string rawRecord)>();
        var processedKeys = new HashSet<string>();

        try
        {
            using (var fileReader = new StreamReader(csvFilePath))
            using (var csvReader = new CsvReader(fileReader, CultureInfo.InvariantCulture))
            {
                csvReader.Read();
                csvReader.ReadHeader();

                int currentRowIndex = 1;
                while (csvReader.Read())
                {
                    currentRowIndex++;
                    var parseResult = _csvParser.ParseCsvRowToTrip(csvReader);

                    if (parseResult.IsValid)
                    {
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
                    else
                    {
                        errorRecords.Add((currentRowIndex, csvReader.Parser.RawRecord));
                    }
                }
            }

            _logger.LogInformation("Writing duplicate trips to file.");
            await _fileWriter.WriteDuplicateTripsAsync(duplicateTrips, duplicatesCsvPath);

            _logger.LogInformation("Writing error records to file.");
            await _fileWriter.WriteErrorRecordsAsync(errorRecords, errorsCsvPath);

            _logger.LogInformation("Inserting valid trips into the database.");
            await _databaseInserter.InsertTripsAsync(validTrips);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while processing data.");
        }
    }
}
