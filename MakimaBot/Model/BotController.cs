using Microsoft.AspNetCore.Mvc;

namespace MakimaBot.Model;

[ApiController]
public class BotController(
    IBotService botService,
    IBucketClient bucketClient,
    BotStateUpdater stateUpdater,
    ITelegramBotClientWrapper telegramBotClientWrapper) : ControllerBase
{
    [HttpPost]
    [Route("")]
    public async Task<IActionResult> ProcessAsync(CancellationToken cancellationToken)
    {
        await stateUpdater.EnsureUpdateAsync(cancellationToken);
        await botService.ProcessAsync(cancellationToken);

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

    [HttpGet]
    [Route("state")]
    public async Task<IActionResult> GetStateAsync(CancellationToken cancellationToken)
    {
        var rawState = await bucketClient.LoadRawStateAsync();

        return new OkObjectResult(rawState);
    }
}
