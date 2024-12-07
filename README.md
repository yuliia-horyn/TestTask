# ETL Project README  

## Project Description  

This project implements a simple ETL (Extract, Transform, Load) process for importing data from a CSV file into an MS SQL database. The data is cleaned, transformed, and bulk inserted into a flat table, following the objectives outlined in the task specifications.  

## Features  

- Extracts data from a CSV file.  
- Transforms data to ensure:  
  - Removal of duplicates based on key.  
  - Conversion of `store_and_fwd_flag` values (`N` to `No`, `Y` to `Yes`).  
  - Whitespace trimming in text fields.  
  - Conversion of `tpep_pickup_datetime` and `tpep_dropoff_datetime` from EST to UTC.  
- Loads transformed data into an MS SQL Server database using an efficient bulk insertion process.  
- Stores removed duplicate records in a separate `duplicates.csv` file.  
- Handles invalid or missing data fields, initializing them with default values where necessary.  

## Prerequisites  

- **Programming Language**: C#  
- **Database**: SQL Server (local).  
- **CSV Parsing Library**: `CsvHelper` (NuGet package).  

## Key Metrics
- **Total Records Inserted**: 29,889
- **Duplicates Removed**: 111 (Saved in duplicates.csv).
- **Records with Errors**: 118 (Initialized with default values). 

## Steps to Run the Project

1. Clone this repository.
2. Run the provided SQL script (`Initial.sql`) to create database and the table.
3. Update the `appsettings.json` file with your SQL Server connection string.
4. Build and run the project from the command line or an IDE.
5. Place the input CSV file in the specified `Data` folder.
6. After execution, the program will:
   - Insert transformed data into the database.
   - Save duplicates into a `duplicates.csv` file in the `Data` folder.

## Assumptions
If the program needs to handle significantly larger data files, such as a 10GB CSV input file, the following optimizations could be implemented:

- Stream Processing:

Instead of reading the entire file into memory at once, can be used a streaming approach (e.g., `CsvHelper` with `StreamReader`) to process rows line by line. This reduces memory usage.

- Batch Processing:

Continue using batch inserts but the batch size can be increased to balance between memory use and the number of database calls.

- Parallel Processing:

The file can be divided into chunks and these chunks would be processed in parallel for faster processing. Needed to ensure the database can handle concurrent writes.

- Use Efficient Data Structures:

To avoid creating large in-memory lists (e.g., `List<T>`). Can be used a generator/yield mechanism to produce rows dynamically.

- SQL Bulk Insert:

Leverage `SqlBulkCopy` as already done but consider increasing the buffer size or using `EnableStreaming` for better performance.


