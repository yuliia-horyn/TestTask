IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'ETL')
BEGIN
    CREATE DATABASE ETL;
END;
GO

USE ETL;
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'TaxiTrips')
BEGIN
    CREATE TABLE TripModel (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        tpep_pickup_datetime DATETIME,
        tpep_dropoff_datetime DATETIME,
        passenger_count INT,
        trip_distance FLOAT,
        store_and_fwd_flag VARCHAR(3),
        PULocationID INT,
        DOLocationID INT,
        fare_amount DECIMAL(10, 2),
        tip_amount DECIMAL(10, 2)
    );
    
    CREATE INDEX idx_PULocation_tip ON TripModel (PULocationID, tip_amount);
    CREATE INDEX idx_trip_distance ON TripModel (trip_distance);
    CREATE INDEX idx_pickup_datetime ON TripModel(tpep_pickup_datetime);
END;
GO