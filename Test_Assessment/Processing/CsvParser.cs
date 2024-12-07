﻿using CsvHelper;
using System;
using System.Globalization;
using Microsoft.Extensions.Logging;
using Test_Assessment.Helpers;
using Test_Assessment.Interfaces;
using Test_Assessment.Model;

public class CsvParser : ICsvParser
{
    private readonly ILogger<CsvParser> _logger;

    public CsvParser(ILogger<CsvParser> logger)
    {
        _logger = logger;
    }

    public (bool IsValid, TripModel Trip) ParseCsvRowToTrip(CsvReader csvReader)
    {
        try
        {
            // Отримання значень з CSV та їх обробка
            var trip = new TripModel
            {
                PickupDatetime = ParseDate(csvReader.GetField(ColumnMappings.PickupDatetime)),
                DropoffDatetime = ParseDate(csvReader.GetField(ColumnMappings.DropoffDatetime)),
                PassengerCount = ParseFieldWithDefault<int>(csvReader, ColumnMappings.PassengerCount, 0),
                TripDistance = ParseFieldWithDefault<double>(csvReader, ColumnMappings.TripDistance, 0.0),
                StoreAndFwdFlag = NormalizeFlag(csvReader.GetField<string>(ColumnMappings.StoreAndFwdFlag)),
                PULocationID = ParseFieldWithDefault<int>(csvReader, ColumnMappings.PULocationID, 0),
                DOLocationID = ParseFieldWithDefault<int>(csvReader, ColumnMappings.DOLocationID, 0),
                FareAmount = ParseFieldWithDefault<decimal>(csvReader, ColumnMappings.FareAmount, 0),
                TipAmount = ParseFieldWithDefault<decimal>(csvReader, ColumnMappings.TipAmount, 0)
            };

            // Валідація об'єкта TripModel
            var validator = new TripModelValidator();
            var validationResult = validator.Validate(trip);

            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed for trip: {Errors}", validationResult.Errors);
                return (false, null);
            }

            return (true, trip);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error parsing CSV row.");
            return (false, null);
        }
    }

    private DateTime ParseDate(string dateValue)
    {
        try
        {
            var estZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            var formats = new[] { "MM/dd/yyyy hh:mm:ss tt" };

            if (!DateTime.TryParseExact(
                    dateValue,
                    formats,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var estDateTime))
            {
                throw new FormatException($"Invalid date format: {dateValue}");
            }

            return TimeZoneInfo.ConvertTimeToUtc(estDateTime, estZone);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Invalid date value: {DateValue}", dateValue);
            throw;
        }
    }

    private T ParseFieldWithDefault<T>(CsvReader csvReader, string columnName, T defaultValue)
    {
        try
        {
            return csvReader.GetField<T>(columnName);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error parsing field {ColumnName}, using default value: {DefaultValue}", columnName, defaultValue);
            return defaultValue;
        }
    }

    private string NormalizeFlag(string flagValue)
    {
        if (string.IsNullOrWhiteSpace(flagValue))
        {
            return "No";
        }

        return flagValue.ToUpper() switch
        {
            "Y" => "Yes",
            "N" => "No",
            _ => "Unknown"
        };
    }
}
