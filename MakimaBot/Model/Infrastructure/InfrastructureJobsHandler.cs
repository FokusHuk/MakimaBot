namespace MakimaBot.Model.Infrastructure;

public class InfrastructureJobsHandler
{
    private readonly IEnumerable<InfrastructureJob> _jobs;
    private readonly IDataContext _dataContext;

    public InfrastructureJobsHandler(
        IEnumerable<InfrastructureJob> jobs,
        IDataContext dataContext)
    {
        _jobs = jobs;
        _dataContext = dataContext;
    }
    
    public async Task TryHandleJobsAsync(CancellationToken cancellationToken)
    {
        try
        {
            foreach (var job in _jobs)
            {
                await TryExecuteJobAsync(job);
            }
        }
        catch (Exception e)
        {
            var errorMessage = $"An error occured while handling infrastructure jobs: {e.Message}";
            Console.WriteLine(errorMessage);
            _dataContext.AddOrUpdateError(DateTime.UtcNow, errorMessage);
            await _dataContext.SaveChangesAsync();
        }
    }

    private async Task TryExecuteJobAsync(InfrastructureJob job)
    {
        try
        {
            await job.ExecuteAsync(_dataContext);
        }
        catch (Exception e)
        {
            var errorMessage = $"An error occured while executing {job.Name}: {e.Message}";
            Console.WriteLine();
            _dataContext.AddOrUpdateError(DateTime.UtcNow, errorMessage);
            await _dataContext.SaveChangesAsync();
        }
    }
}
