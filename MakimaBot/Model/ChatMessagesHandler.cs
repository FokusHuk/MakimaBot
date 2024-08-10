using Telegram.Bot;
using Telegram.Bot.Types;

namespace MakimaBot.Model;

public class ChatMessagesHandler
{
    private readonly TelegramBotClient _telegramBotClient;
    private readonly DataContext _dataContext;

    private const int UpdateMessagesLimit = 25;

    public ChatMessagesHandler(TelegramBotClient telegramBotClient, DataContext dataContext)
    {
        _telegramBotClient = telegramBotClient;
        _dataContext = dataContext;
    }

    public async Task TryHandleUpdatesAsync(CancellationToken cancellationToken)
    {
        try
        {
            await HandleUpdatesAsync(cancellationToken);
        }
        catch (Exception e)
        {
            var errorMessage = $"An error occured while handling updates: {e.Message}";
            Console.WriteLine(errorMessage);
            _dataContext.AddOrUpdateError(DateTime.UtcNow, errorMessage);
            await _dataContext.SaveChangesAsync();
        }
    }

    private async Task HandleUpdatesAsync(CancellationToken cancellationToken)
    {
        var messagesOffset = 0;
        var updates = await _telegramBotClient.GetUpdatesAsync(
            offset: messagesOffset,
            limit: UpdateMessagesLimit,
            cancellationToken: cancellationToken);

        if (!updates.Any())
        {
            return;
        }

        foreach (var update in updates)
        {
            await TryHandleMessagesAsync(update, cancellationToken);
            messagesOffset = update.Id + 1;
            await _telegramBotClient.GetUpdatesAsync(offset: messagesOffset, cancellationToken: cancellationToken);
        }
    }

    private async Task TryHandleMessagesAsync(Update update, CancellationToken cancellationToken)
    {
        try
        {
            await HandleMessageAsync(update, cancellationToken);
        }
        catch (Exception e)
        {
            var errorMessage = $"An error occured while handling message: {e.Message}";
            Console.WriteLine(errorMessage);
            _dataContext.AddOrUpdateError(DateTime.UtcNow, errorMessage);
            await _dataContext.SaveChangesAsync();
        }
    }
    
    private async Task HandleMessageAsync(Update update, CancellationToken cancellationToken)
    {
        if (update.Message is not { } message)
            return;
    
        var chatId = message.Chat.Id;
    
        if (message.Sticker is { SetName: { } } sticker &&
            sticker.SetName.Equals("makimapak", StringComparison.InvariantCultureIgnoreCase))
        {
            if (sticker.Emoji == "😤")
            {
                await _telegramBotClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: "❤️",
                    replyToMessageId: message.MessageId,
                    cancellationToken: cancellationToken);
            }
    
            return;
        }
    
        if (message.From != null)
        {
            var chatState = _dataContext.GetChatStateById(chatId);

            if (chatState is null)
            {
                await ProcessUnknownChatMessageAsync(message, chatId, cancellationToken);
            }
            else
            {
                await ProcessTrustedChatAsync(message, chatState, cancellationToken);
            }
        }
    }

    private async Task ProcessTrustedChatAsync(Message message, ChatState chatState, CancellationToken cancellationToken)
    {
        if (chatState.EventsState.ActivityStatistics.IsEnabled)
        {
            var chatActivityStatistics = chatState.EventsState.ActivityStatistics.Statistics;
            if (chatActivityStatistics.ContainsKey(message.From.Id))
                chatActivityStatistics[message.From.Id]++;
            else
                chatActivityStatistics[message.From.Id] = 1;

            await _dataContext.SaveChangesAsync();
        }

        var random = new Random();
        if (random.Next(10) < 1)
        {
            var reactions = new []
            {
                "Я все вижу 👀",
                "Хватит сюда писать",
                "...",
                "Не надо слов, просто погавкай 🐶",
                "Я сейчас ливну отсюда",
                "Ахахаххахахаха",
                "До вечера 🌙"
            };
                
            await _telegramBotClient.SendTextMessageAsync(
                chatState.ChatId,
                reactions[random.Next(reactions.Length)],
                cancellationToken: cancellationToken);
        }
    }

    private async Task ProcessUnknownChatMessageAsync(Message message, long chatId, CancellationToken cancellationToken)
    {
        // await _telegramBotClient.SendTextMessageAsync(
        //     chatId: chatId,
        //     text: "Привет! Я Макима.\nИ мне запрещают общаться с незнакомцами. Но если очень хочется, можете написать хозяину :)\nhttps://t.me/akima_yooukie",
        //     cancellationToken: cancellationToken);
        
        _dataContext.AddUnknownMessage(
            DateTime.UtcNow,
            chatId,
            message.Text,
            message.From?.Username ?? message.From?.FirstName ?? message.From?.LastName ?? message.From?.Id.ToString());
        
        await _dataContext.SaveChangesAsync();
    }
}
