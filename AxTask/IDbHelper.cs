namespace AxTask;

public interface IDbHelper
{
    void SaveLogRecords(List<LogRecord> logRecords);
    List<LogRecord> GetLogRecords();
    IQueryable<LogRecord> DoSQL(string sql);
    List<LogRecord> SearchBySubstring(string substring);
    List<LogRecord> GetBySeverity(int severity);
    void SaveQueryResults(QueryResult queryResult);
    void Clear();
}