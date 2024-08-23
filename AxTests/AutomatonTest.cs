using AxTask;
using Microsoft.Data.Sqlite;
using Moq;

namespace AxTests;

[TestFixture]
[TestOf(typeof(Automaton))]
public class AutomatonTest
{
    private Automaton automaton;
    private Mock<IDbHelper> dbHelperMock;

    [SetUp]
    public void Setup()
    {
        dbHelperMock = new Mock<IDbHelper>();
        automaton = new Automaton(dbHelperMock.Object, new string[] { "test.log" }, "SELECT * FROM Logs", "Column1", "test", "output.json", "1");
    }

    [Test]
    public void ReadFile_ValidPath_ReturnsLines()
    {
        // Arrange
        string path = "test.txt";
        File.WriteAllLines(path, new[] { "Line1", "Line2", "Line3" });

        // Act
        IEnumerable<string> lines = automaton.ReadFile(path);

        // Assert
        Assert.That(lines, Is.Not.Null);
        Assert.That(lines.Count(), Is.EqualTo(3));

        // Cleanup
        File.Delete(path);
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
    public void ParseLines_ValidLines_ParsesRecords()
    {
        // Arrange
        IEnumerable<string> lines = new List<string>
            {
                "Name,Age,City",
                "John,25,New York",
                "Jane,30,Los Angeles"
            };

        // Act
        automaton.ParseLines(lines);

        // Assert
        Assert.That(automaton.LogRecords, Is.Not.Null);
        Assert.That(automaton.LogRecords.Count, Is.EqualTo(2));
        Assert.That(automaton.LogRecords[0].RecordValues["Name"], Is.EqualTo("John"));
        Assert.That(automaton.LogRecords[0].RecordValues["Age"], Is.EqualTo("25"));
        Assert.That(automaton.LogRecords[0].RecordValues["City"], Is.EqualTo("New York"));
        Assert.That(automaton.LogRecords[1].RecordValues["Name"], Is.EqualTo("Jane"));
        Assert.That(automaton.LogRecords[1].RecordValues["Age"], Is.EqualTo("30"));
        Assert.That(automaton.LogRecords[1].RecordValues["City"], Is.EqualTo("Los Angeles"));
    }   

    [Test]
    public void RemoveDuplicates_DuplicateRecords_RemovesDuplicates()
    {
        // Arrange
        List<LogRecord> logRecords = new()
        {
                new LogRecord
                {
                    RecordValues = new Dictionary<string, string>
                    {
                        { "Name", "John" },
                        { "Age", "25" },
                        { "City", "New York" }
                    }
                },
                new LogRecord
                {
                    RecordValues = new Dictionary<string, string>
                    {
                        { "Name", "John" },
                        { "Age", "25" },
                        { "City", "New York" }
                    }
                },
                new LogRecord
                {
                    RecordValues = new Dictionary<string, string>
                    {
                        { "Name", "Jane" },
                        { "Age", "30" },
                        { "City", "Los Angeles" }
                    }
                }
            };

        // Act
        List<LogRecord> result = automaton.RemoveDuplicates(logRecords);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result[0].RecordValues["Name"], Is.EqualTo("John"));
        Assert.That(result[0].RecordValues["Age"], Is.EqualTo("25"));
        Assert.That(result[0].RecordValues["City"], Is.EqualTo("New York"));
        Assert.That(result[1].RecordValues["Name"], Is.EqualTo("Jane"));
        Assert.That(result[1].RecordValues["Age"], Is.EqualTo("30"));
        Assert.That(result[1].RecordValues["City"], Is.EqualTo("Los Angeles"));
    }

    [Test]
    public void PerformQuery_ValidQuery_SavesResults()
    {
        // Arrange
        var logRecords = new List<LogRecord>
            {
                new() { RecordValues = new Dictionary<string, string> { { "Name", "John" }, { "Age", "25" }, { "City", "New York" } } },
                new() { RecordValues = new Dictionary<string, string> { { "Name", "Jane" }, { "Age", "30" }, { "City", "Los Angeles" } } }
            };

        dbHelperMock.Setup(db => db.DoSql(It.IsAny<string>())).Returns(logRecords.AsQueryable());

        // Act
        automaton.PerformQuery();

        // Assert
        Assert.That(automaton.Results, Is.Not.Null);
        Assert.That(automaton.Results.Count, Is.EqualTo(2));

        string json = File.ReadAllText("output.json");
        Assert.That(json, Is.Not.Null.Or.Empty);

        // Cleanup
        File.Delete("output.json");
    }
    [Test]
    public void AlertBySeverity_NoRecordsWithSeverity_PrintsNoRecordsFound()
    {
        // Arrange
        dbHelperMock.Setup(db => db.GetBySeverity(It.IsAny<int>())).Returns(new List<LogRecord>());

        var consoleOutput = new StringWriter();
        Console.SetOut(consoleOutput);

        // Act
        automaton.AlertBySeverity(3);

        // Assert
        string expectedOutput = "No records found with severityValue 3" + Environment.NewLine;
        Assert.That(consoleOutput.ToString(), Is.EqualTo(expectedOutput));
    }
}
