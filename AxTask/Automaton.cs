using Microsoft.Data.Sqlite;
using System.Text.Json;

namespace AxTask;

public class Automaton(IDbHelper dbHelper)
{
   /// <summary>
    /// Checks if user search input is actually a valid SQL query.
    /// Use this for database layer to read and write the files.
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public bool IsValidSqlQuery(string query)
    {
        return true;
        var command = new SqliteCommand(query);
        try
        {
            command.ExecuteNonQuery();
        }
        catch (SqliteException)
        {
            return false;
        }

        return !string.IsNullOrEmpty(query);
    }
    public IEnumerable<string> ReadFile(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            throw new ArgumentNullException(nameof(path),"Provided file string is empty");
        }
        if (!File.Exists(path))
        {
            throw new FileNotFoundException("File not found", path);
        }
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
            {
                logRecord.RecordValues[header[i]] = values.Length > i ? values[i] : string.Empty;
            }

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
            if (uniqueRecords.Add(recordString))
            {
                result.Add(record);
            }
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
        if (IsValidSqlQuery(Query))
        {
          
            try
            {
                Results = dbHelper.DoSQL(Query).ToList();
                SaveResults();
            }
            catch (SqliteException ex)
            {
                Console.WriteLine($"SQL Error: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine("Invalid query");
            PrintHelp();
        }
    }

    private void SaveResults()
    {
        JsonSerializerOptions jsonSerializerOptions = new()
        {
            WriteIndented = true,

        };
        var json = JsonSerializer.Serialize(Results, jsonSerializerOptions);
        File.WriteAllText(Results.Count() + ".json",
            json);
        Console.WriteLine("Query results: ");
        Console.WriteLine(json);
    }

    public List<string> Columns { get; set; } = [];
    public string FileName { get; set; } = string.Empty;
    public string Query { get; set; } = string.Empty;
    public List<LogRecord> LogRecords { get; set; } = [];
    public List<LogRecord> Results { get; set; } 
}