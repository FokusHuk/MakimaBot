#nullable disable
namespace MakimaBot.Model;

public interface IGptClient
{
    Task<GptTextCompletionResponse> SendAsync(string promt);
}
