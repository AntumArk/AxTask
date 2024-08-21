namespace AxTask
{
    internal class Program
    {       
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            // todo read the input file
            var automaton = new Automaton();
            var areArgsCorrect=automaton.ParseArgs(args);
            var fileName= automaton.FileName;

            if (!areArgsCorrect)
            {
                Console.WriteLine("Invalid arguments");
                return;
            }
            var logLines = automaton.ReadFile(fileName);
            // todo parse the input file. Input file is a list of strings, and is tabulated to columns.

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
