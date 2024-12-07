using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Test_Assessment.Helpers;
using Test_Assessment.Interfaces;
using Test_Assessment.Model;

public class DatabaseInserter : IDatabaseInserter
{
    private readonly string _connectionString;
    private readonly ILogger<DatabaseInserter> _logger;

    public DatabaseInserter(IConfiguration configuration, ILogger<DatabaseInserter> logger)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
        _logger = logger;
    }

    public async Task InsertTripsAsync(List<TripModel> trips, int batchSize = 500)
    {
        try
        {
            for (int i = 0; i < trips.Count; i += batchSize)
            {
                var batch = trips.GetRange(i, Math.Min(batchSize, trips.Count - i));
                await InsertBatchAsync(batch);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inserting trips into the database.");
        }
    }

    private async Task InsertBatchAsync(List<TripModel> trips)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            using (var bulkCopy = new SqlBulkCopy(connection))
            {
                bulkCopy.DestinationTableName = "[ETL].[dbo].[TripModel]";
                bulkCopy.ColumnMappings.Add(ColumnMappings.PickupDatetime, ColumnMappings.PickupDatetime);
                bulkCopy.ColumnMappings.Add(ColumnMappings.DropoffDatetime, ColumnMappings.DropoffDatetime);
                bulkCopy.ColumnMappings.Add(ColumnMappings.PassengerCount, ColumnMappings.PassengerCount);
                bulkCopy.ColumnMappings.Add(ColumnMappings.TripDistance, ColumnMappings.TripDistance);
                bulkCopy.ColumnMappings.Add(ColumnMappings.StoreAndFwdFlag, ColumnMappings.StoreAndFwdFlag);
                bulkCopy.ColumnMappings.Add(ColumnMappings.PULocationID, ColumnMappings.PULocationID);
                bulkCopy.ColumnMappings.Add(ColumnMappings.DOLocationID, ColumnMappings.DOLocationID);
                bulkCopy.ColumnMappings.Add(ColumnMappings.FareAmount, ColumnMappings.FareAmount);
                bulkCopy.ColumnMappings.Add(ColumnMappings.TipAmount, ColumnMappings.TipAmount);

                var dataTable = CreateDataTable(trips);
                await bulkCopy.WriteToServerAsync(dataTable);
            }
        }
    }

    private DataTable CreateDataTable(List<TripModel> trips)
    {
        var table = new DataTable();
        table.Columns.Add(ColumnMappings.PickupDatetime, typeof(DateTime));
        table.Columns.Add(ColumnMappings.DropoffDatetime, typeof(DateTime));
        table.Columns.Add(ColumnMappings.PassengerCount, typeof(int));
        table.Columns.Add(ColumnMappings.TripDistance, typeof(double));
        table.Columns.Add(ColumnMappings.StoreAndFwdFlag, typeof(string));
        table.Columns.Add(ColumnMappings.PULocationID, typeof(int));
        table.Columns.Add(ColumnMappings.DOLocationID, typeof(int));
        table.Columns.Add(ColumnMappings.FareAmount, typeof(decimal));
        table.Columns.Add(ColumnMappings.TipAmount, typeof(decimal));

        foreach (var trip in trips)
        {
            table.Rows.Add(
                trip.PickupDatetime,
                trip.DropoffDatetime,
                trip.PassengerCount,
                trip.TripDistance,
                trip.StoreAndFwdFlag,
                trip.PULocationID,
                trip.DOLocationID,
                trip.FareAmount,
                trip.TipAmount
            );
        }

        return table;
    }
}
