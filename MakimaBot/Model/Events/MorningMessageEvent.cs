using System.Globalization;

namespace MakimaBot.Model.Events;

public class MorningMessageEvent : DailyMessageEvent
{
    protected override TimeSpan EventTimeStartUtc { get => _morningTimeStartUtc; }
    protected override TimeSpan EventTimeEndUtc { get => _morningTimeEndUtc; }
    protected override string[] EventMessages { get => _morningMessages; }

    private readonly TimeSpan _morningTimeStartUtc =
        DateTime.Parse("2023-01-01 05:00:00", CultureInfo.InvariantCulture).TimeOfDay;
    
    private readonly TimeSpan _morningTimeEndUtc =
        DateTime.Parse("2023-01-01 07:25:00", CultureInfo.InvariantCulture).TimeOfDay;
    
    private readonly string[] _morningMessages = {
        "Доброе утро ❤️",
        "Доброе утро, красивые 🕊",
        "Good morning, sunshines :3",
        "Утро... спать хочется",
        "Хорошего дня, я дальше спать 😴",
        "Чудесного дня, друзья 🤤",
        "Доброе утро, друзья!"
    };
}
