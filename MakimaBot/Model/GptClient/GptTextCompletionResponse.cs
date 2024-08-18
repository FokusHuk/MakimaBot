#nullable disable
using System.Text.Json.Serialization;

namespace MakimaBot.Model;

public class GptTextCompletionResponse
{
    [JsonPropertyName("result")]
    public GptTextCompletionResult Result { get; set; }
}

public class GptTextCompletionResult
{
    [JsonPropertyName("alternatives")]
    public List<GptTextCompletionAlternative> Alternatives { get; set; }

    [JsonPropertyName("usage")]
    public GptTextCompletionUsage Usage { get; set; }

    [JsonPropertyName("modelVersion")]
    public string ModelVersion { get; set; }
}

public class GptTextCompletionAlternative
{
    [JsonPropertyName("message")]
    public GptTextCompletionResponseMessage Message { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; }
}

public class GptTextCompletionResponseMessage
{
    [JsonPropertyName("role")]
    public string Role { get; set; }

    [JsonPropertyName("text")]
    public string Text { get; set; }
}

public class GptTextCompletionUsage
{
    [JsonPropertyName("inputTextTokens")]
    public string InputTextTokens { get; set; }

    [JsonPropertyName("completionTokens")]
    public string CompletionTokens { get; set; }

    [JsonPropertyName("totalTokens")]
    public string TotalTokens { get; set; }
}
