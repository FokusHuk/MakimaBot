#nullable disable

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MakimaBot.Model;

public class BotError : ValidatableObject
{   
    [Required]
    [JsonPropertyName("message")]
    public string Message { get; set; }

    [Required]
    [JsonPropertyName("lastSeenDateTimeUtc")]
    public DateTime LastSeenDateTimeUtc { get; set; }

    [Required]
    [JsonPropertyName("count")]
    public int Count { get; set; }

    protected override IEnumerable<CompositeValidationResult> ValidateCompositeProperties() => [];
}