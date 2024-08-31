#nullable disable

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MakimaBot.Model;

public class ScheduledMessageEventState : ValidatableObject
{
    [Required]
    [JsonPropertyName("isEnabled")]
    public bool IsEnabled { get; set; }

    [Required]
    [JsonPropertyName("lastTimeStampUtc")]
    public DateTime LastTimeStampUtc { get; set; }

    [Required]
    [JsonPropertyName("nextStartTimeStampUtc")]
    public DateTime NextStartTimeStampUtc { get; set; }

    protected override IEnumerable<CompositeValidationResult> ValidateCompositeProperties() => [];
}
