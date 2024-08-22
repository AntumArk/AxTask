using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AxTask;

public class LogContext : DbContext
{
    public DbSet<LogRecord> LogRecords { get; set; }
    public DbSet<QueryResult> QueryResults { get; set; }
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

        modelBuilder.Entity<QueryResult>()
            .ToTable("QueryResults")
            .Property(q => q.JsonData)
            .HasConversion(
                v => v,
                v => v); 
    }

    public void EnsureLogRecordsTableCreated()
    {
        Database.Migrate();
    }
}
