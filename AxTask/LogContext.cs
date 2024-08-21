using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace AxTask;

public class LogContext:DbContext
{
    public DbSet<LogRecord> LogRecords { get; set; } 
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=logs.db");
    }
    private readonly JsonSerializerOptions options = new(JsonSerializerDefaults.Web);
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<LogRecord>()
            .Property(b => b.RecordValues)
            .HasConversion(
                v => JsonSerializer.Serialize(v, options),
                v => JsonSerializer.Deserialize<Dictionary<string, string>>(v,options));
    }
}