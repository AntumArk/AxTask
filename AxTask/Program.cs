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
            // todo add search by substring for each column. Return column not found if the substring is not found in the column. The query can be in any syntax, like SQL of smth like that.

            // todo process the input file
            // todo write the output file in json format.

            // Bonus
            // todo boolean operator in query
            // todo add multiple file support
            // todo add log count value in resulting JSON


            // Extra Bonus
            // todo remove the duplicates from the output file
            // todo add a command line argument to specify the output file name
            // todo database layer to read and write the files
            // todo implement send alert based on severity
        }
    }
}
