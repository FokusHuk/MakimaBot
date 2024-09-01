using System.ComponentModel.DataAnnotations;

namespace MakimaBot.Model.Config;

public class ApplicationOptions
{
    [Required]
    public string Version { get; init; }
}
