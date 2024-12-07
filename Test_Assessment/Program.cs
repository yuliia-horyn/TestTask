using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Test_Assessment.Interfaces;

class Program
{
    static async Task Main(string[] args)
    {
        var serviceProvider = ConfigureServices();
        var dataProcessor = serviceProvider.GetService<IDataProcessor>();

        if (dataProcessor == null)
        {
            Console.WriteLine("DataProcessor is not configured.");
            return;
        }

        await dataProcessor.ProcessDataAsync();
        Console.WriteLine("Data processing complete.");
    }

    private static IServiceProvider ConfigureServices()
    {
        string path = Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..");
        string absolutePath = Path.GetFullPath(path);

        var builder = new ConfigurationBuilder()
                            .SetBasePath(absolutePath)
                            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                            .AddUserSecrets<Program>();

        IConfiguration configuration = builder.Build();


        string connectionString = configuration.GetConnectionString("DefaultConnection");
        string inputCsvPath = Path.Combine(absolutePath, "Data", "sample-cab-data.csv");
        string duplicatesCsvPath = Path.Combine(absolutePath, "Data", "duplicates.csv");
        string errorCsvPath = Path.Combine(absolutePath, "Data", "errors.csv");

        // Configure logging
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        var services = new ServiceCollection();

        // Add configuration
        services.AddSingleton<IConfiguration>(configuration);

        // Add logging
        services.AddLogging(config => config.AddSerilog());

        // Register services
        services.AddSingleton<ICsvParser, CsvParser>();
        services.AddSingleton<IDatabaseInserter, DatabaseInserter>();
        services.AddSingleton<IFileWriter, FileWriter>();
        services.AddSingleton<IDataProcessor, DataProcessor>();

        return services.BuildServiceProvider();
    }
}
