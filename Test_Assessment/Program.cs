using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Test_Assessment;
using Test_Assessment.Helpers;
using Test_Assessment.Interfaces;
using Test_Assessment.Queries;


class Program
{
    static async Task Main(string[] args)
    {
        var serviceProvider = ConfigureServices();
        var dataProcessor = serviceProvider.GetService<IDataProcessor>();
        var databaseQueries = serviceProvider.GetService<IDatabaseQueries>();

        if (dataProcessor == null)
        {
            Console.WriteLine("DataProcessor is not configured.");
            return;
        }

        await dataProcessor.ProcessDataAsync();
        await databaseQueries.ExecuteQueriesAsync();
        Console.WriteLine("Data processing complete.");
    }

    private static IServiceProvider ConfigureServices()
    {
        var configuration = new ConfigurationBuilder()
                            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                            .AddUserSecrets<Program>()
                            .Build();

        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddLogging(config => config.AddSerilog());

        services.AddSingleton(sp =>
        {
            var config = sp.GetRequiredService<IConfiguration>();
            return new DatabaseSettings
            {
                ConnectionString = config.GetConnectionString("DefaultConnection"),
                DestinationTableName = config["DatabaseSettings:DestinationTableName"]
            };
        });

        var filePathSettings = configuration.GetSection("FilePaths").Get<FilePathSettings>();
        services.AddSingleton(filePathSettings);
        services.AddSingleton<PathService>();

        services.AddSingleton<IDataParser, DataParser>();
        services.AddSingleton<IDatabaseInserter, DatabaseInserter>();
        services.AddSingleton<IFileWriter, FileWriter>();
        services.AddSingleton<IDataProcessor, DataProcessor>();
        services.AddSingleton<IDatabaseQueries, DatabaseQueries>();

        return services.BuildServiceProvider();
    }
}
