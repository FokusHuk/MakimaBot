#nullable disable

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MakimaBot.Model;

public class UnknownChatMessage : ValidatableObject
{
    [Required]
    [JsonPropertyName("sentDateTimeUtc")]
    public DateTime SentDateTimeUtc { get; set; }

    [Required]
    [JsonPropertyName("chatId")]
    public long ChatId { get; set; }

    [Required]
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [Required]
    [JsonPropertyName("message")]
    public string Message { get; set; }

    protected override IEnumerable<CompositeValidationResult> ValidateCompositeProperties() => [];
}
