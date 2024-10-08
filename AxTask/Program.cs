﻿using Microsoft.Extensions.Configuration;

namespace AxTask;

internal class Program
{
    private static void Main(string[] args)
    {
        // Set up configuration to read from command-line arguments
        var configuration = new ConfigurationBuilder()
            .AddCommandLine(args)
            .Build();

        // Read the command-line arguments
        var files = configuration["files"]?.Split(" ");
        var query = configuration["query"];
        var severity = configuration["alert"];
        var column = configuration["column"];
        var substring = configuration["substring"];
        var outputFileName = configuration["output"];
        var removeDuplicates = args.Contains("-r");


        if (AreArgumentsInvalid(files, query, column, substring, outputFileName,severity)) return;

        var dbHelper = new DbHelper(new LogContext());
        dbHelper.Clear(); // Clear the database before starting

        var automaton = new Automaton(dbHelper, files!, query, column, substring, outputFileName!, severity);
                
        try
        {
            automaton.Execute(removeDuplicates);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

    }

    private static bool AreArgumentsInvalid(string[]? files, string? query, string? column, string? substring,
        string? outputFileName, string? severity)
    {
        var invalidArguments = new List<string>();

        if (files == null || files.Length == 0)
        {
            invalidArguments.Add("--files");
        }

        if (!IsQueryProvided(query, column, substring))
        {
            invalidArguments.Add("--query or --column and --substring");
        }

        if (!IsOutputFilePathCorrect(outputFileName))
        {
            invalidArguments.Add("--output");
        }

        if (!IsSeverityANumber(severity))
        {
            invalidArguments.Add("--alert");
        }

        if (invalidArguments.Count == 0) return false;
        Console.WriteLine("Invalid arguments: " + string.Join(", ", invalidArguments));
        PrintHelp();
        return true;

    }

    /// <summary>
    /// Checks if the severity is a number, empty or null string is also valid
    /// </summary>
    /// <param name="severity"></param>
    /// <returns></returns>
    private static bool IsSeverityANumber(string? severity)
    {
        return string.IsNullOrEmpty(severity) || int.TryParse(severity, out _);
    }

    private static bool IsOutputFilePathCorrect(string? outputFileName)
    {
        if (string.IsNullOrEmpty(outputFileName)) return false;
        var outputDirectory = Path.GetDirectoryName(outputFileName);
        return string.IsNullOrEmpty(outputDirectory) || Directory.Exists(outputDirectory);
    }

    private static bool IsQueryProvided(string? query, string? column, string? substring)
    {
        var isQueryProvided = !string.IsNullOrEmpty(query);
        var isColumnProvided = !string.IsNullOrEmpty(column) && !string.IsNullOrEmpty(substring);
        return isColumnProvided || isQueryProvided;
    }

    public static void PrintHelp()
    {
        Console.WriteLine("Usage: AxTask [options]");
        Console.WriteLine();
        Console.WriteLine("Options:");
        Console.WriteLine("  --files \"file1 file2 ...\"      Specifies the list of CSV files to process (required).");
        Console.WriteLine("  --query \"<query>\"              Specifies the SQL query to execute (required).");
        Console.WriteLine("  --column \"<column>\"            Specifies the column to search (required).");
        Console.WriteLine("  --substring \"<substring>\"      Specifies the substring to search for in the specified column (required).");
        Console.WriteLine("  --output <filename>             Specifies the output file name (required).");
        Console.WriteLine("  --alert <severity>              Specifies the alert severity level (optional).");
        Console.WriteLine("  -r                              Removes duplicate log records (optional).");
        Console.WriteLine();
        Console.WriteLine("Note: You must provide either the --query argument or both the --column and --substring arguments.");
        Console.WriteLine();
        Console.WriteLine("Examples:");
        Console.WriteLine("  AxTask --files \"file1.csv file2.csv\" --query \"SELECT * FROM LogRecords WHERE RecordValues->>'signatureId' LIKE '%4608%'\"");
        Console.WriteLine("  AxTask --files \"file1.csv file2.csv\" --column \"signatureId\" --substring \"4608\"");
        Console.WriteLine("  AxTask --files \"file1.csv file2.csv\" --query \"SELECT * FROM LogRecords\" --alert 10 --output \"output.json\" -r");
    }


}
