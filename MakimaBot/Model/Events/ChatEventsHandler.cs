using Telegram.Bot;

namespace MakimaBot.Model.Events;

public class ChatEventsHandler
{
    private readonly TelegramBotClient _telegramBotClient;
    private readonly ICollection<IChatEvent> _chatEvents;

    public ChatEventsHandler(TelegramBotClient telegramBotClient, ICollection<IChatEvent> chatEvents)
    {
        _telegramBotClient = telegramBotClient;
        _chatEvents = chatEvents;
    }
    
    public async Task TryHandleEventsAsync(BotState state)
    {
        try
        {
            await HandleEventsAsync(state);
        }
        catch (Exception e)
        {
            var errorMessage = $"An error occured while handling chats events: {e.Message}";
            Console.WriteLine(errorMessage);
            state.Errors.Add(new BotError
            {
                CreationDateTimeUtc = DateTime.UtcNow,
                Message = errorMessage
            });
            state.WasUpdated = true;
        }
    }
    
    private async Task HandleEventsAsync(BotState state)
    {
        foreach (var chat in state.Chats)
        {
            await TryHandleChatEventsAsync(state, chat);
        }
    }

    private async Task TryHandleChatEventsAsync(BotState state, ChatState chat)
    {
        try
        {
            await HandleChatEventsAsync(chat);
        }
        catch (Exception e)
        {
            var errorMessage = $"An error occured while handling chat \"{chat.Name}\" event: {e.Message}";
            Console.WriteLine(errorMessage);
            state.Errors.Add(new BotError
            {
                CreationDateTimeUtc = DateTime.UtcNow,
                Message = errorMessage
            });
            state.WasUpdated = true;
        }
    }
    
    private async Task HandleChatEventsAsync(ChatState chat)
    {
        var telegramChat = await _telegramBotClient.GetChatAsync(chat.ChatId);
        
        if (telegramChat.Username != null && telegramChat.Username.Contains(chat.Name, StringComparison.OrdinalIgnoreCase)
            || telegramChat.Title != null && telegramChat.Title.Contains(chat.Name, StringComparison.OrdinalIgnoreCase))
        {
            foreach (var chatEvent in _chatEvents)
            {
                if (chatEvent.ShouldLaunch(chat))
                {
                    await chatEvent.HandleEventAsync(_telegramBotClient, chat);
                }
            }
        }
    }
}
