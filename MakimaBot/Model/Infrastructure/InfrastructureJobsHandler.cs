namespace MakimaBot.Model.Infrastructure;

public class InfrastructureJobsHandler(
    IEnumerable<InfrastructureJob> jobs,
    IDataContext dataContext)
{   
    public async Task TryHandleJobsAsync(CancellationToken cancellationToken)
    {
        try
        {
            foreach (var job in jobs)
            {
                await TryExecuteJobAsync(job);
            }
        }
        catch (Exception e)
        {
            var errorMessage = $"An error occured while handling infrastructure jobs: {e.Message}";
            Console.WriteLine(errorMessage);
            dataContext.AddOrUpdateError(DateTime.UtcNow, errorMessage);
            await dataContext.SaveChangesAsync();
        }
    }

    private async Task TryExecuteJobAsync(InfrastructureJob job)
    {
        try
        {
            await job.ExecuteAsync();
        }
        catch (Exception e)
        {
            var errorMessage = $"An error occured while executing {job.Name}: {e.Message}";
            Console.WriteLine();
            dataContext.AddOrUpdateError(DateTime.UtcNow, errorMessage);
            await dataContext.SaveChangesAsync();
        }
    }
}
