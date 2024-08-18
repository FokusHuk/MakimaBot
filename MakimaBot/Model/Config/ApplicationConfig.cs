using System.Text.Json.Serialization;

namespace MakimaBot.Model.Config;

public class ApplicationConfig
{
    [JsonPropertyName("telegramBotToken")]
    public required string TelegramBotToken { get; init; }

    [JsonPropertyName("bucketConfig")]
    public required BucketConfig BucketConfig { get; init; }

    [JsonPropertyName("gptConfig")]
    public required GptConfig GptConfig { get; init; }
}

public class BucketConfig
{
    [JsonPropertyName("accessKeyId")]
    public required string AccessKeyId { get; init; }

    [JsonPropertyName("secretAccessKey")]
    public required string SecretAccessKey { get; init; }

    [JsonPropertyName("bucketName")]
    public required string BucketName { get; init; }

    [JsonPropertyName("serviceUrl")]
    public required string ServiceUrl { get; init; }

    [JsonPropertyName("stateFileName")]
    public required string StateFileName { get; init; }
}

public class GptConfig
{
    [JsonPropertyName("url")]
    public required string Url { get; set; }

    [JsonPropertyName("apiKey")]
    public required string ApiKey { get; set; }

    [JsonPropertyName("modelUrl")]
    public required string ModelUrl { get; set; }
}
