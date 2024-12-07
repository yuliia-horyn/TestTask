using Microsoft.Extensions.Logging;
using System.Data.SqlClient;
using System.Data;
using Test_Assessment.Helpers;
using Test_Assessment.Interfaces;
using Test_Assessment.Model;
using Test_Assessment;

public class DatabaseInserter(DatabaseSettings settings, ILogger<DatabaseInserter> logger) : IDatabaseInserter
{
    private readonly string _connectionString = settings.ConnectionString
            ?? throw new ArgumentNullException(nameof(settings.ConnectionString), "Connection string is not configured.");
    private readonly string _destinationTableName = settings.DestinationTableName
            ?? throw new ArgumentNullException(nameof(settings.DestinationTableName), "Destination table name is not configured.");

    public async Task InsertTripsAsync(List<TripModel> trips, int batchSize = 500, CancellationToken cancellationToken = default)
    {
        if (trips == null || !trips.Any())
        {
            logger.LogWarning("The trips list is null or empty.");
            return;
        }

        if (batchSize <= 0) throw new ArgumentException("Batch size must be greater than zero.", nameof(batchSize));

        logger.LogInformation("Inserting {Count} trips in batches of {BatchSize}.", trips.Count, batchSize);

        for (int i = 0; i < trips.Count; i += batchSize)
        {
            var batch = trips.GetRange(i, Math.Min(batchSize, trips.Count - i));
            try
            {
                await InsertBatchAsync(batch, cancellationToken);
                logger.LogInformation("Batch starting at index {Index} successfully inserted.", i);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error inserting batch starting at index {Index}.", i);
                throw;
            }
        }
    }

    private async Task InsertBatchAsync(List<TripModel> trips, CancellationToken cancellationToken)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        using var bulkCopy = ConfigureBulkCopy(connection);
        var dataTable = CreateDataTable(trips);
        await bulkCopy.WriteToServerAsync(dataTable, cancellationToken);
    }

    private SqlBulkCopy ConfigureBulkCopy(SqlConnection connection)
    {
        var bulkCopy = new SqlBulkCopy(connection)
        {
            DestinationTableName = _destinationTableName
        };

        bulkCopy.ColumnMappings.Add(ColumnMappings.PickupDatetime, ColumnMappings.PickupDatetime);
        bulkCopy.ColumnMappings.Add(ColumnMappings.DropoffDatetime, ColumnMappings.DropoffDatetime);
        bulkCopy.ColumnMappings.Add(ColumnMappings.PassengerCount, ColumnMappings.PassengerCount);
        bulkCopy.ColumnMappings.Add(ColumnMappings.TripDistance, ColumnMappings.TripDistance);
        bulkCopy.ColumnMappings.Add(ColumnMappings.StoreAndFwdFlag, ColumnMappings.StoreAndFwdFlag);
        bulkCopy.ColumnMappings.Add(ColumnMappings.PULocationID, ColumnMappings.PULocationID);
        bulkCopy.ColumnMappings.Add(ColumnMappings.DOLocationID, ColumnMappings.DOLocationID);
        bulkCopy.ColumnMappings.Add(ColumnMappings.FareAmount, ColumnMappings.FareAmount);
        bulkCopy.ColumnMappings.Add(ColumnMappings.TipAmount, ColumnMappings.TipAmount);

        return bulkCopy;
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
