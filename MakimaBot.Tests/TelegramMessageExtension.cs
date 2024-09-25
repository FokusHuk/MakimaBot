using Telegram.Bot.Types;

namespace MakimaBot.Tests;

public static class TelegramMessageExtension
{
    public static Message WithSticker(this Message message, string setName, string emoji)
    {
        message.Sticker = new Sticker{
            SetName = setName,
            Emoji = emoji
        };
        return message;
    }

    public static Message WithSender(this Message message, long userId)
    {
        message.From = new User{
            Id = userId
        };
        return message;
    }
    
    public static Message WithText(this Message message, string text)
    {
        message.Text = text;
        return message;
    }
}
