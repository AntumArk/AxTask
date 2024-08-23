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

        if (files == null || files.Length == 0 || string.IsNullOrEmpty(query))
        {
            PrintHelp();
            return;
        }

        Console.WriteLine("Hello, World!");
        IDbHelper dbHelper = new DbHelper(new LogContext());
        dbHelper.Clear();
        var automaton = new Automaton(dbHelper);

        if (!automaton.ParseArgs(args))
        {
            Console.WriteLine("Invalid arguments");
            return;
        }

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
    public static void PrintHelp()
    {
        Console.WriteLine("Usage: AxTask --files \"file1 file2\" ... --query \"<query>\" [--alert <severity>]");
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