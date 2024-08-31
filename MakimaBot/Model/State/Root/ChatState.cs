#nullable disable

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MakimaBot.Model;

public class ChatState : ValidatableObject
{
    [Required]
    [JsonPropertyName("chatId")]
    public long ChatId { get; set; }

    [Required]
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [Required]
    [JsonPropertyName("eventsState")]
    public EventsState EventsState { get; set; }

    protected override IEnumerable<CompositeValidationResult> ValidateCompositeProperties() =>
        EventsState.Validate();
}
