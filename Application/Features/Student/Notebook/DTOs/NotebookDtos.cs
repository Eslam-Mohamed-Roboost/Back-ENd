using API.Helpers.Attributes;
using System.Text.Json.Serialization;

namespace API.Application.Features.Student.Notebook.DTOs;

public class NotebookEntryDto
{
    [JsonConverter(typeof(LongAsStringConverter))]
    public long Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty; // HTML from rich text editor
    public DateTime CreatedDate { get; set; }
    public DateTime LastModifiedDate { get; set; }
    [JsonConverter(typeof(LongAsStringConverter))]
    public long SubjectId { get; set; }
    public List<string> Tags { get; set; } = new();
    public bool IsFavorite { get; set; }
}

public class SaveNotebookEntryRequest
{
    [JsonConverter(typeof(LongAsStringConverter))]
    public long? Id { get; set; } // Null for new entry
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    [JsonConverter(typeof(LongAsStringConverter))]
    public long SubjectId { get; set; }
    public List<string> Tags { get; set; } = new();
    public bool IsFavorite { get; set; }
}
