namespace MakimaBot.Model;

public interface IDateTimeProvider
{
    DateTime UtcNow();
}

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow() => DateTime.UtcNow;
}
