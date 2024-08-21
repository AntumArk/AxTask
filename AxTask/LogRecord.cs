using System.ComponentModel.DataAnnotations;

namespace AxTask;

public record LogRecord
{
    [Key]
    public ulong Id{ get; set; }
    public Dictionary<string, string> RecordValues { get; set; }
}