using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Data.Sqlite;

namespace AxTask;

public class Automaton(IDbHelper dbHelper)
{
    private readonly JsonSerializerOptions options = new()
    {
        WriteIndented = true
    };

    public List<string> Columns { get; set; } = [];
    public string FileName { get; set; } = string.Empty;
    public string Query { get; set; } = string.Empty;
    public List<LogRecord> LogRecords { get; set; } = [];
    public List<LogRecord> Results { get; set; } = [];

    public void CheckIfColumnExists(string sql)
    {
        var regex = new Regex(@"RecordValues->>'(?<column>[^']*)'");
        var matches = regex.Matches(sql);
        var columns = LogRecords.First().RecordValues.Keys;

        foreach (Match match in matches)
        {
            var column = match.Groups["column"].Value;
            if (!columns.Contains(column)) throw new InvalidOperationException($"Column '{column}' does not exists");
        }
    }

    public IEnumerable<string> ReadFile(string path)
    {
        if (string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path), "Provided file string is empty");
        if (!File.Exists(path)) throw new FileNotFoundException("File not found", path);
        var lines = File.ReadAllLines(path);
        return lines.ToList();
    }

    public bool ParseArgs(string[] args)
    {
        if (args.Length != 2)
        {
            PrintHelp();
            return false;
        }

        FileName = args[0];
        Query = args[1];
        return true;
    }

    public void PrintHelp()
    {
        Console.WriteLine("Usage: AxTask <filename> \"<query>\"");
    }

    public void ParseFile(IEnumerable<string> lines)
    {
        var linesList = lines.ToList();
        var header = GetHeaderColumns(linesList);

        foreach (var line in linesList.Skip(1))
        {
            var values = line.Split(',');
            var logRecord = new LogRecord
            {
                RecordValues = new Dictionary<string, string>()
            };

            for (var i = 0; i < header.Length; i++)
                logRecord.RecordValues[header[i]] = values.Length > i ? values[i] : string.Empty;

            LogRecords.Add(logRecord);
        }
    }

    public List<LogRecord> RemoveDuplicates(List<LogRecord> logRecords)
    {
        var uniqueRecords = new HashSet<string>();
        var result = new List<LogRecord>();

        foreach (var record in logRecords)
        {
            var recordString = JsonSerializer.Serialize(record.RecordValues);
            if (uniqueRecords.Add(recordString)) result.Add(record);
        }

        return result;
    }

    private string[] GetHeaderColumns(IEnumerable<string> lines)
    {
        var header = lines.First().Split(',');
        Columns = header.ToList();
        return header;
    }

    public void PerformQuery()
    {
        try
        {
            Results = dbHelper.DoSQL(Query).ToList();
            SaveResults();
        }
        catch (SqliteException ex)
        {
            Console.WriteLine($"SQL Error: {ex.Message}");
            PrintHelp();
        }
    }

    private void SaveResults()
    {
        JsonSerializerOptions jsonSerializerOptions = new()
        {
            WriteIndented = true
        };

        var json = JsonSerializer.Serialize(new
        {
            searchQuery = Query,
            resultsCount = Results.Count,
            result = Results
        }, jsonSerializerOptions);

        File.WriteAllText(Results.Count + ".json", json);

        Console.WriteLine("Query results: ");
        Console.WriteLine(json);
        QueryResult queryResult = new()
        {
            JsonData = json,
            TimeOfEntry = DateTime.Now
        };
        dbHelper.SaveQueryResults(queryResult);
    }
    // ...

    public void AlertBySeverity(int severity)
    {
        var results = dbHelper.GetBySeverity(severity);
        if (results.Count > 0)
        {
            Console.BackgroundColor = ConsoleColor.Red; // Set console background color to red
            Console.WriteLine($"Found {results.Count} records with severity {severity} or higher");


            foreach (var result in results)
            {
                Console.BackgroundColor = ConsoleColor.DarkRed; // Set console background color to dark red
                Console.WriteLine(JsonSerializer.Serialize(result.RecordValues, options));
                Console.WriteLine();
                Console.ResetColor(); // Reset console background color to default
            }
        }
        else
        {
            Console.WriteLine($"No records found with severity {severity}");
        }
    }
}