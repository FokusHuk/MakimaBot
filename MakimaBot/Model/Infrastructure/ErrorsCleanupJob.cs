﻿namespace MakimaBot.Model.Infrastructure;

public class ErrorsCleanupJob : InfrastructureJob
{
    public override string Name => $"{nameof(ErrorsCleanupJob)}";

    public override async Task ExecuteAsync(DataContext dataContext)
    {
        var currentDateTimeUtc = DateTime.UtcNow;
        var errorsCleanupThreshold = TimeSpan.FromHours(24);
        
        var errorsToRemove = new List<BotError>();
        var errors = dataContext.GetAllErrors().ToList();
        
        foreach (var botError in errors)
        {
            if (currentDateTimeUtc - botError.CreationDateTimeUtc > errorsCleanupThreshold)
                errorsToRemove.Add(botError);
        }
        
        foreach (var errorToRemove in errorsToRemove)
        {
            errors.Remove(errorToRemove);
        }
        
        if (errorsToRemove.Any())
        {
            dataContext.UpdateErrors(errors);
            await dataContext.SaveChangesAsync();
        }
    }
}
