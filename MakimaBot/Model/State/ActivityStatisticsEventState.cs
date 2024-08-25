#nullable disable

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MakimaBot.Model;

public class ActivityStatisticsEventState : ValidatableObject
{
    [Required]
    [JsonPropertyName("isEnabled")]
    public bool IsEnabled { get; set; }

    [Required]
    [JsonPropertyName("lastTimeStampUtc")]
    public DateTime LastTimeStampUtc { get; set; }

    [Required]
    [JsonPropertyName("statistics")]
    public Dictionary<long, int> Statistics { get; set; }

    protected override IEnumerable<CompositeValidationResult> ValidateCompositeProperties() => [];
}
