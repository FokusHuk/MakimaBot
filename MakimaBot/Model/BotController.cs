#nullable disable
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;

namespace MakimaBot.Model;

[ApiController]
public class BotController : ControllerBase
{
    private readonly IBotService _botService;
    private readonly IHttpClientFactory _httpClientFactory;

    public BotController(IBotService botService, IHttpClientFactory httpClientFactory)
    {
        _botService = botService;
        _httpClientFactory = httpClientFactory;
    }

    [HttpPost]
    [Route("")]
    public async Task<IActionResult> ProcessAsync(CancellationToken cancellationToken)
    {
        await _botService.ProcessAsync(cancellationToken);

        return Ok();
    }

    [HttpPost]
    [Route("health")]
    public IActionResult CheckHealth(CancellationToken cancellationToken)
    {
        return new JsonResult(new
        {
            Status = "Healthy"
        });
    }

    [HttpPost]
    [Route("regexp")]
    public IActionResult ParseRegexp(CancellationToken cancellationToken)
    {
        var message = "@makima_daily_bot gpt";
        var pattern = @"^@makima_daily_bot\s*([a-z]*)\s*(.*)$";

        var match = Regex.Matches(message, pattern, RegexOptions.IgnoreCase);
        var commandName = match.First().Groups[1].Value;
        var promt = match.First().Groups[2].Value;

        return new JsonResult(new {
            CommandName = commandName,
            Promt = promt
        });
    }

    [HttpPost]
    [Route("test")]
    public async Task<IActionResult> Test(CancellationToken cancellationToken)
    {
        var httpClient = _httpClientFactory.CreateClient();
        httpClient.BaseAddress = new Uri("https://llm.api.cloud.yandex.net/foundationModels/v1/completion");
        httpClient.DefaultRequestHeaders.Add("Authorization", "Api-Key AQVN3Fgt23eVRBcRFoRER6PYg1yu0mlY-V_oUPnP");

        var requestBody = new GptTextCompletionRequest
        {
            ModelUri = "gpt://b1gnt622or5en89ramit/yandexgpt-lite/latest",
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
                    Text = "Ты мой собеседник. Поддерживай разговор"
                },
                new ()
                {
                    Role = "user",
                    Text = "Сколько лет планете Земля?"
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

        var deserializedContent = JsonSerializer.Deserialize<GptTextCompletionResponse>(contentString);

        return new JsonResult(deserializedContent);
    }   
}
