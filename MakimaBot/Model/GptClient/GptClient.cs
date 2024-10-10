#nullable disable
using System.Text.Json;
using MakimaBot.Model.Config;
using Microsoft.Extensions.Options;

namespace MakimaBot.Model;

public class GptClient : IGptClient
{
    private readonly IHttpClientFactory _httpClientFactory;

    private readonly IOptions<GptOptions> _gptOptions;

    private const int MaxTokens= 250;
    private readonly string DefaultRole = $"Тебя зовут Макима. Ты добрая и общительная девушка. Ты мой собеседник. Поддерживай разговор.";

    public GptClient(IHttpClientFactory httpClientFactory, IOptions<GptOptions> gptOptions)
    {
        _httpClientFactory = httpClientFactory;
        _gptOptions = gptOptions;
    }

    public async Task<GptTextCompletionResponse> SendAsync(string promt)
    {
        var httpClient = CreateClient();

        var requestBody = new GptTextCompletionRequest
        {
            ModelUri = _gptOptions.Value.ModelUrl,
            CompletionOptions = new GptTextCompletionOptions
            {
                Stream = false,
                Temperature = 0.4,
                MaxTokens = MaxTokens
            },
            Messages = new List<GptTextCompletionRequestMessage>()
            {
                new ()
                {
                    Role = "system",
                    Text = DefaultRole
                },
                new ()
                {
                    Role = "user",
                    Text = promt
                }
            }
        };

        var body = JsonSerializer.Serialize(requestBody);

        var message = new HttpRequestMessage(HttpMethod.Post, "")
        {
            Content = new StringContent(body)
        };

        var response = await httpClient.SendAsync(message);

        var contentString = await response.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<GptTextCompletionResponse>(contentString);
    }

    private HttpClient CreateClient()
    {
        var httpClient = _httpClientFactory.CreateClient();
        httpClient.BaseAddress = new Uri(_gptOptions.Value.Url);
        httpClient.DefaultRequestHeaders.Add("Authorization", $"Api-Key {_gptOptions.Value.ApiKey}");
        return httpClient;
    }
}
