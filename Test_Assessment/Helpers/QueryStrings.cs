namespace Test_Assessment.Helpers
{
    public static class QueryStrings
    {
        public const string HighestAverageTipLocation = @"
            SELECT TOP 1 PULocationID, AVG(tip_amount) AS AverageTipAmount
            FROM [ETL].[dbo].[TripModel]
            GROUP BY PULocationID
            ORDER BY AverageTipAmount DESC;
        ";

        public const string Top100LongestFaresByDistance = @"
            SELECT TOP 100 Id, trip_distance
            FROM [ETL].[dbo].[TripModel]
            ORDER BY trip_distance DESC;
        ";

        public const string Top100LongestFaresByTime = @"
            SELECT TOP 100 Id, DATEDIFF(SECOND, tpep_pickup_datetime, tpep_dropoff_datetime) AS DurationInSeconds
            FROM [ETL].[dbo].[TripModel]
            ORDER BY DurationInSeconds DESC;
        ";

        public const string SearchTripsByPULocationId = @"
            SELECT Id, tpep_pickup_datetime, tpep_dropoff_datetime, PULocationID
            FROM [ETL].[dbo].[TripModel]
            WHERE PULocationID = @PULocationId;
        ";
    }
}
