using Microsoft.AspNetCore.Mvc;

namespace MakimaBot.Model;

[ApiController]
public class BotController : ControllerBase
{
    private readonly IBotService _botService;
    private readonly IBucketClient _bucketClient;
    private readonly BotStateUpdater _stateUpdater;

    public BotController(
        IBotService botService,
        IBucketClient bucketClient,
        BotStateUpdater stateUpdater)
    {
        _botService = botService;
        _bucketClient = bucketClient;
        _stateUpdater = stateUpdater;
    }

    [HttpPost]
    [Route("")]
    public async Task<IActionResult> ProcessAsync(CancellationToken cancellationToken)
    {
        await _stateUpdater.EnsureUpdateAsync(cancellationToken);
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

    [HttpGet]
    [Route("state")]
    public async Task<IActionResult> GetStateAsync(CancellationToken cancellationToken)
    {
        var rawState = await _bucketClient.LoadRawStateAsync();

        return new OkObjectResult(rawState);
    }
}
