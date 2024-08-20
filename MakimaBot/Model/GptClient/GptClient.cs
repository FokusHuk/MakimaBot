#nullable disable
using System.Text.Json;
using MakimaBot.Model.Config;

namespace MakimaBot.Model;

public class GptClient : IGptClient
{
    private readonly IHttpClientFactory _httpClientFactory;

    private readonly GptConfig _configuration;

    private const string DefaultRole = " Тебя зовут Макима. Ты добрая и общительная девушка. Ты мой собеседник. Поддерживай разговор";

    public GptClient(IHttpClientFactory httpClientFactory, GptConfig configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    public async Task<GptTextCompletionResponse> SendAsync(string promt)
    {
        var httpClient = CreateClient();

        var requestBody = new GptTextCompletionRequest
        {
            ModelUri = _configuration.ModelUrl,
            CompletionOptions = new GptTextCompletionOptions
            {
                Stream = false,
                Temperature = 0.7,
                MaxTokens = 100
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
        httpClient.BaseAddress = new Uri(_configuration.Url);
        httpClient.DefaultRequestHeaders.Add("Authorization", $"Api-Key {_configuration.ApiKey}");
        return httpClient;
    }
}
