using System.ComponentModel.DataAnnotations;

namespace AxTask;

public class QueryResult
{
    [Key]
    public int Id { get; set; }
    [Required]
    public DateTime TimeOfEntry { get; set; }
    [Required]
    public string JsonData { get; set; } = string.Empty;
}