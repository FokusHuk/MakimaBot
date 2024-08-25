#nullable disable

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MakimaBot.Model;

public class InfrastructureState : ValidatableObject
{
    [Required]
    [JsonPropertyName("errors")]
    public ICollection<BotError> Errors { get; set; }

    [Required]
    [JsonPropertyName("unknownChatMessages")]
    public ICollection<UnknownChatMessage> UnknownChatsMessages { get; set; }

    protected override IEnumerable<CompositeValidationResult> ValidateCompositeProperties() =>
        Errors
            .SelectMany(error => error.Validate())
            .Concat(UnknownChatsMessages
                .SelectMany(message => message.Validate()));
}