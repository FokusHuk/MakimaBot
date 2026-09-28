using System.ComponentModel.DataAnnotations;

namespace MakimaBot.Model.Config;

public class TelegramOptions
{
    public static readonly string SectionName = "telegramConfig";
    public static readonly string ProxySecretHeader = "X-Proxy-Secret";

    [Required]
    public string Token { get; init; }

    public string? BaseUrl { get; init; }

    public string? ProxySecret { get; init; }
}
