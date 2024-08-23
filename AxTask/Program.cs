using Microsoft.Extensions.Configuration;

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

        if (files == null || 
            files.Length == 0 || 
            !IsQueryProvided(query,column,substring))
        {
            PrintHelp();
            return;
        }

        Console.WriteLine("Hello, World!");
        IDbHelper dbHelper = new DbHelper(new LogContext());
        dbHelper.Clear();
        var automaton = new Automaton(dbHelper);


        var logLines = automaton.ReadFile(automaton.FileName);
        automaton.ParseFile(logLines);
        try
        {
            automaton.CheckIfColumnExists(automaton.Query);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        automaton.LogRecords = automaton.RemoveDuplicates(automaton.LogRecords);
        dbHelper.SaveLogRecords(automaton.LogRecords);
        automaton.PerformQuery();
        automaton.AlertBySeverity(10);

        // Bonus
        // todo add multiple file support
        // todo add simple column search


        // todo add a command line argument to specify the output file name
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
        Console.WriteLine("  --files \"file1 file2 ...\"      Specifies the list of CSV files to process.");
        Console.WriteLine("  --query \"<query>\"              Specifies the SQL query to execute.");
        Console.WriteLine("  --column \"<column>\"            Specifies the column to search.");
        Console.WriteLine("  --substring \"<substring>\"      Specifies the substring to search for in the specified column.");
        Console.WriteLine("  --alert <severity>              Specifies the alert severity level (optional).");
        Console.WriteLine();
        Console.WriteLine("Note: You must provide either the --query argument or both the --column and --substring arguments.");
        Console.WriteLine();
        Console.WriteLine("Examples:");
        Console.WriteLine("  AxTask --files \"file1.csv file2.csv\" --query \"SELECT * FROM LogRecords WHERE RecordValues->>'signatureId' LIKE '%4608%'\"");
        Console.WriteLine("  AxTask --files \"file1.csv file2.csv\" --column \"signatureId\" --substring \"4608\"");
        Console.WriteLine("  AxTask --files \"file1.csv file2.csv\" --query \"SELECT * FROM LogRecords\" --alert 10");
    }
    public static bool ParseArgs(string[] args)
    {
        if (args.Length != 2)
        {
            PrintHelp();
            return false;
        }

        //FileName = args[0];
        //Query = args[1];
        return true;
    }
}