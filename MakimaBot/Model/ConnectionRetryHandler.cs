namespace MakimaBot.Model;

public class ConnectionRetryHandler(int maxAttempts) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        for (var attempt = 1; ; attempt++)
        {
            try
            {
                return await base.SendAsync(request, cancellationToken);
            }
            catch (Exception ex) when (attempt < maxAttempts && IsConnectFailure(ex, cancellationToken))
            {
                Console.WriteLine($"Connection to {request.RequestUri?.Host} failed " +
                                  $"(attempt {attempt}/{maxAttempts}): {ex.InnerException?.Message ?? ex.Message}");
            }
        }
    }

    private static bool IsConnectFailure(Exception ex, CancellationToken cancellationToken) => ex switch
    {
        OperationCanceledException { InnerException: TimeoutException } => !cancellationToken.IsCancellationRequested,
        HttpRequestException { HttpRequestError: HttpRequestError.ConnectionError } => true,
        _ => false
    };
}
