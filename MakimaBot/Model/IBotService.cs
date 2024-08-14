namespace MakimaBot.Model;

public interface IBotService
{
    Task ProcessAsync(CancellationToken cancellationToken);
}
