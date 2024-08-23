using AxTask;

namespace AxTests;

[TestFixture]
[TestOf(typeof(Automaton))]
public class AutomatonTest
{
    private Automaton automaton;

    [SetUp]
    public void Setup()
    {
        IDbHelper dbHelper = new DbHelper(); // Create an instance of IDbHelper
        automaton = new Automaton(dbHelper); // Pass the IDbHelper instance to the Automaton constructor
    }

  


    [Test]
    public void ReadFile_ValidPath_ReturnsLines()
    {
        // Arrange
        string path = "test.txt";

        // Act
        IEnumerable<string> lines = automaton.ReadFile(path);

        // Assert
        Assert.That(lines, Is.Not.Null);
        Assert.That(lines.Count(), Is.EqualTo(3));
    }

    [Test]
    public void ReadFile_EmptyPath_ThrowsArgumentNullException()
    {
        // Arrange
        string path = "";

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => automaton.ReadFile(path));
    }

    [Test]
    public void ReadFile_NonExistentPath_ThrowsFileNotFoundException()
    {
        // Arrange
        string path = "nonexistent.txt";

        // Act & Assert
        Assert.Throws<FileNotFoundException>(() => automaton.ReadFile(path));
    }

    [Test]
    public void ParseArgs_ValidArgs_ReturnsTrue()
    {
        // Arrange
        string[] args = { "test.txt", "SELECT * FROM Customers" };

        // Act
        bool result = automaton.ParseArgs(args);

        // Assert
        Assert.That(result, Is.True);
        Assert.That(automaton.FileName, Is.EqualTo("test.txt"));
        Assert.That(automaton.Query, Is.EqualTo("SELECT * FROM Customers"));
    }

    [Test]
    public void ParseArgs_InvalidArgs_ReturnsFalse()
    {
        // Arrange
        string[] args = { "test.txt" };

        // Act
        bool result = automaton.ParseArgs(args);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public void PrintHelp_WritesHelpMessage()
    {
        // Arrange
        var consoleOutput = new StringWriter();
        Console.SetOut(consoleOutput);

        // Act
        automaton.PrintHelp();

        // Assert
        Assert.That(consoleOutput.ToString().Trim(), Is.EqualTo("Usage: AxTask <filename> <query>"));
    }

    [Test]
    public void ParseFile_ValidLines_ParsesRecords()
    {
        // Arrange
        IEnumerable<string> lines = new List<string>
            {
                "Name, Age, City",
                "John, 25, New York",
                "Jane, 30, Los Angeles"
            };

        // Act
        automaton.ParseLines(lines);

        // Assert
        // Add your assertions here
    }

    [Test]
    public void PerformQuery_NotImplemented_ThrowsNotImplementedException()
    {
        // Arrange & Act & Assert
        Assert.Throws<NotImplementedException>(() => automaton.PerformQuery());
    }
}
