using System.ComponentModel.DataAnnotations;

namespace MakimaBot.Model.Config;

public class TelegramOptions
{
    public static readonly string SectionName = "telegramConfig";

    [Required]
    public string Token { get; init; }
}
