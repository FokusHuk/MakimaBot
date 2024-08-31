using System.ComponentModel.DataAnnotations;

namespace MakimaBot.Model.Config;

public class GptOptions
{
    public static readonly string SectionName = "gptConfig";

    [Required]
    public string Url { get; set; }

    [Required]
    public string ApiKey { get; set; }

    [Required]
    public string ModelUrl { get; set; }
}
