namespace MakimaBot.Model.Infrastructure;

public class InfrastructureJobsHandler
{
    private readonly ICollection<InfrastructureJob> _jobs;
    private readonly DataContext _dataContext;

    public InfrastructureJobsHandler(
        ICollection<InfrastructureJob> jobs,
        DataContext dataContext)
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
                TryExecuteJob(job);
            }
        }
        catch (Exception e)
        {
            var errorMessage = $"An error occured while handling infrastructure jobs: {e.Message}";
            Console.WriteLine(errorMessage);
            _dataContext.AddError(DateTime.UtcNow, errorMessage);
        }
    }

    private void TryExecuteJob(InfrastructureJob job)
    {
        try
        {
            job.ExecuteAsync(_dataContext);
        }
        catch (Exception e)
        {
            var errorMessage = $"An error occured while executing {job.Name}: {e.Message}";
            Console.WriteLine();
            _dataContext.AddError(DateTime.UtcNow, errorMessage);
        }
    }
}
