using Microsoft.AspNetCore.Mvc;

namespace MakimaBot.Model;

[ApiController]
public class BotController : ControllerBase
{
    private readonly IBotService _botService;

    public BotController(IBotService botService)
    {
        _botService = botService;
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
}
