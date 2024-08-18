#nullable disable
using System.Text.Json.Serialization;

namespace MakimaBot.Model;

public class GptTextCompletionRequest
{
    [JsonPropertyName("modelUri")]
    public string ModelUri { get; set; }

    [JsonPropertyName("completionOptions")]
    public GptTextCompletionOptions CompletionOptions { get; set; }

    [JsonPropertyName("messages")]
    public List<GptTextCompletionRequestMessage> Messages { get; set; }
}

public class GptTextCompletionOptions
{
    [JsonPropertyName("stream")]
    public bool Stream { get; set; }

    [JsonPropertyName("temperature")]
    public double Temperature { get; set; }

    [JsonPropertyName("maxTokens")]
    public long MaxTokens { get; set; }
}

public class GptTextCompletionRequestMessage
{
    [JsonPropertyName("role")]
    public string Role { get; set; }

    [JsonPropertyName("text")]
    public string Text { get; set; }
}
