using Microsoft.AspNetCore.Mvc;

namespace MakimaBot.Model;

[ApiController]
public class BotController : ControllerBase
{
    private readonly IBotService _botService;
    private readonly IDataContext _dataContext;
    private readonly BotStateUpdater _stateUpdater;

    public BotController(
        IBotService botService,
        IDataContext dataContext,
        BotStateUpdater stateUpdater)
    {
        _botService = botService;
        _dataContext = dataContext;
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
        await _dataContext.ConfigureAsync();

        return new JsonResult(_dataContext.State);
    }
}
