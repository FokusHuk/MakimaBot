#nullable disable

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MakimaBot.Model;

public class BotState : ValidatableObject
{
    [Required]
    [JsonPropertyName("chats")]
    public ICollection<ChatState> Chats { get; set; }

    [Required]
    [JsonPropertyName("infrastructure")]
    public InfrastructureState Infrastructure { get; set; }

    protected override IEnumerable<CompositeValidationResult> ValidateCompositeProperties() =>
        Chats
            .SelectMany(chat => chat.Validate())
            .Concat(Infrastructure.Validate());
}
