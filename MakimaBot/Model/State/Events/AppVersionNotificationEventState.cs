#nullable disable

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MakimaBot.Model;

public class AppVersionNotificationEventState : ValidatableObject
{
    [Required]
    [JsonPropertyName("isEnabled")]
    public bool IsEnabled { get; set; }

    [Required]
    [JsonPropertyName("lastNotifiedAppVersionId")]
    public int LastNotifiedAppVersionId { get; set; }

    protected override IEnumerable<CompositeValidationResult> ValidateCompositeProperties() => [];
}