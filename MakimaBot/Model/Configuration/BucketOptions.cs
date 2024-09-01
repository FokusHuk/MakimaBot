using System.ComponentModel.DataAnnotations;

namespace MakimaBot.Model.Config;

public class BucketOptions
{
    public static readonly string SectionName = "bucketConfig";

    [Required]
    public string AccessKeyId { get; init; }

    [Required]
    public string SecretAccessKey { get; init; }

    [Required]
    public string BucketName { get; init; }

    [Required]
    public string ServiceUrl { get; init; }

    [Required]
    public string StateFileName { get; init; }
}
