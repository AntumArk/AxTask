using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace AxTask;

public record LogRecord
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [JsonIgnore]
    public ulong Id { get; set; }
    [Required]
    public Dictionary<string, string> RecordValues { get; set; }
}
