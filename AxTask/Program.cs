namespace AxTask
{
    internal class Program
    {       
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            IDbHelper dbHelper = new DbHelper(new LogContext());
            dbHelper.Clear();
            var automaton = new Automaton(dbHelper);
            var areArgsCorrect=automaton.ParseArgs(args);
            var fileName= automaton.FileName;

            if (!areArgsCorrect)
            {
                Console.WriteLine("Invalid arguments");
                return;
            }

            var logLines = automaton.ReadFile(fileName);
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
    }
}
