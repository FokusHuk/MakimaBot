using Telegram.Bot;
using Telegram.Bot.Types;

namespace MakimaBot.Model;

public class ChatMessagesHandler
{
    private readonly TelegramBotClient _telegramBotClient;
    
    private const int UpdateMessagesLimit = 25;

    public ChatMessagesHandler(TelegramBotClient telegramBotClient)
    {
        _telegramBotClient = telegramBotClient;
    }

    public async Task TryHandleUpdatesAsync(BotState state, CancellationToken cancellationToken)
    {
        try
        {
            await HandleUpdatesAsync(state, cancellationToken);
        }
        catch (Exception e)
        {
            var errorMessage = $"An error occured while handling updates: {e.Message}";
            Console.WriteLine(errorMessage);
            state.Errors.Add(new BotError
            {
                CreationDateTimeUtc = DateTime.UtcNow,
                Message = errorMessage
            });
            state.WasUpdated = true;
        }
    }

    private async Task HandleUpdatesAsync(BotState state, CancellationToken cancellationToken)
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
            await TryHandleMessagesAsync(state, update, cancellationToken);
            messagesOffset = update.Id + 1;
            await _telegramBotClient.GetUpdatesAsync(offset: messagesOffset, cancellationToken: cancellationToken);
        }
    }

    private async Task TryHandleMessagesAsync(BotState state, Update update, CancellationToken cancellationToken)
    {
        try
        {
            await HandleMessageAsync(state, update, cancellationToken);
        }
        catch (Exception e)
        {
            var errorMessage = $"An error occured while handling message: {e.Message}";
            Console.WriteLine(errorMessage);
            state.Errors.Add(new BotError
            {
                CreationDateTimeUtc = DateTime.UtcNow,
                Message = errorMessage
            });
            state.WasUpdated = true;
        }
    }
    
    private async Task HandleMessageAsync(BotState state, Update update, CancellationToken cancellationToken)
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
            var chatConfig = state.Chats.SingleOrDefault(chat => chat.ChatId == chatId);

            if (chatConfig is null)
            {
                await ProcessUnknownChatMessageAsync(message, state, chatId, cancellationToken);
            }
            else
            {
                await ProcessTrustedChatAsync(message, chatConfig, cancellationToken);
            }
        }
    }

    private async Task ProcessTrustedChatAsync(Message message, ChatState chatConfig, CancellationToken cancellationToken)
    {
        if (chatConfig.EventsState.ActivityStatistics.IsEnabled)
        {
            var chatActivityStatistics = chatConfig.EventsState.ActivityStatistics.Statistics;
            if (chatActivityStatistics.ContainsKey(message.From.Id))
                chatActivityStatistics[message.From.Id]++;
            else
                chatActivityStatistics[message.From.Id] = 1;

            chatConfig.WasUpdated = true;
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
                chatConfig.ChatId,
                reactions[random.Next(reactions.Length)],
                cancellationToken: cancellationToken);
        }
    }

    private async Task ProcessUnknownChatMessageAsync(Message message, BotState state, long chatId, CancellationToken cancellationToken)
    {
        // await _telegramBotClient.SendTextMessageAsync(
        //     chatId: chatId,
        //     text: "Привет! Я Макима.\nИ мне запрещают общаться с незнакомцами. Но если очень хочется, можете написать хозяину :)\nhttps://t.me/akima_yooukie",
        //     cancellationToken: cancellationToken);
        
        state.UnknownChatsMessages.Add(new UnknownChatMessage
        {
            SentDateTimeUtc = DateTime.UtcNow,
            ChatId = chatId,
            Message = message.Text ?? "Unknown message",
            Name = message.From?.Username ?? message.From?.FirstName ?? message.From?.LastName ?? message.From?.Id.ToString() ?? "Unknown user"
        });
        state.WasUpdated = true;
    }
}
