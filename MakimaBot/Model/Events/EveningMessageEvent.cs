using System.Globalization;

namespace MakimaBot.Model.Events;

public class EveningMessageEvent : DailyMessageEvent
{
    protected override TimeSpan EventTimeStartUtc { get => _eveningTimeStartUtc; }
    protected override TimeSpan EventTimeEndUtc { get => _eveningTimeEndUtc; }
    protected override string[] EventMessages { get => _eveningMessages; }

    private readonly TimeSpan _eveningTimeStartUtc =
        DateTime.Parse("2023-01-01 20:30:00", CultureInfo.InvariantCulture).TimeOfDay;

    private readonly TimeSpan _eveningTimeEndUtc =
        DateTime.Parse("2023-01-01 22:00:00", CultureInfo.InvariantCulture).TimeOfDay;

    private readonly string[] _eveningMessages = {
        "Спокойной ночи, пусть сны будут волшебными 🌙✨",
        "Ночь окутала мир своим спокойствием. Сладких снов! 🌌",
        "Good night, sleep tight, and dream of wonderful things 🌜💫",
        "Пусть ночь принесет умиротворение и отдых. Спокойной ночи! 🌟",
        "Сладких и спокойных снов, пусть ночь будет уютной 😴💤",
        "Закрывай глаза и погружайся в мир сновидений. Спокойной ночи! 💤🌙",
        "Ночная тишина и звезды сопровождают ваш сон. Спокойной ночи, дорогие! 🌠"
    };
}
