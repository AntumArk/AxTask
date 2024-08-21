namespace AxTask;

public class Automaton
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
        throw new NotImplementedException();
        return true;
    }
    public List<string> ReadFile(string path)
    {
        throw new NotImplementedException();
        return new List<string>();
    }

    public bool ParseArgs(string[] args)
    {
        throw new NotImplementedException();
    }

    public void PerformQuery()
    {
        throw new NotImplementedException();
    }
}