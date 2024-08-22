using Microsoft.EntityFrameworkCore;

namespace AxTask;

public class DbHelper : IDbHelper
{
    private readonly LogContext logContext;

    public DbHelper(LogContext logContext)
    {
        this.logContext = logContext;
        this.logContext.EnsureLogRecordsTableCreated();
    }

    public void SaveLogRecords(List<LogRecord> logRecords)
    {
        logContext.LogRecords.AddRange(logRecords);
        logContext.SaveChanges();
    }
    public void SaveQueryResults(QueryResult queryResult)
    {
        logContext.QueryResults.Add(queryResult);
        logContext.SaveChanges();
    }
    public List<LogRecord> GetLogRecords()
    {
        return logContext.LogRecords.ToList();
    } 

    public IQueryable<LogRecord> DoSQL(string sql)
    {       
        try
        {
            return logContext.LogRecords.FromSqlRaw(sql);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public List<LogRecord> SearchBySubstring(string substring)
    {
        return logContext.LogRecords.Where(record => record.RecordValues.Any(value => value.Value.Contains(substring))).ToList();
    }

    public List<LogRecord> GetBySeverity(int severity)
    {
        var records = logContext.LogRecords
            .AsEnumerable()
            .Where(record => record.RecordValues.Keys.Contains("severity"));
        return records.Where(record =>
            !string.IsNullOrEmpty(record.RecordValues["severity"]) &&
            int.TryParse(record.RecordValues["severity"], out var severityValue) &&
            severityValue >= severity
        ).ToList();
    }

    public void Clear()
    {
        logContext.LogRecords.RemoveRange(logContext.LogRecords);
        logContext.SaveChanges();
    }
}
