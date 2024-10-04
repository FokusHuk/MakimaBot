namespace MakimaBot.Model.Infrastructure;

public abstract class InfrastructureJob
{
    public abstract string Name { get; }
    
    public abstract Task ExecuteAsync(IDataContext dataContext);
}
