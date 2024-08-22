using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AxTask;

public class LogContext : DbContext
{
    public DbSet<LogRecord> LogRecords { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=logs.db");
    }
    private readonly JsonSerializerOptions options = new()
    {
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
        WriteIndented = true
    };

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<LogRecord>()
            .ToTable("LogRecords") // Add this line to specify the table name
            .Property(b => b.RecordValues)
            .HasConversion(
                v => JsonSerializer.Serialize(v, options),
                v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, options));
    }

    public void EnsureLogRecordsTableCreated()
    {
        Database.EnsureCreated();
    }
}
