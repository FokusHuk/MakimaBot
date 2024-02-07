﻿using Telegram.Bot;

namespace MakimaBot.Model.Events;

public class ChatEventsHandler
{
    private readonly TelegramBotClient _telegramBotClient;
    private readonly ICollection<IChatEvent> _chatEvents;
    private readonly DataContext _dataContext;

    public ChatEventsHandler(
        TelegramBotClient telegramBotClient,
        ICollection<IChatEvent> chatEvents,
        DataContext dataContext)
    {
        _telegramBotClient = telegramBotClient;
        _chatEvents = chatEvents;
        _dataContext = dataContext;
    }
    
    public async Task TryHandleEventsAsync()
    {
        try
        {
            await HandleEventsAsync();
        }
        catch (Exception e)
        {
            var errorMessage = $"An error occured while handling chats events: {e.Message}";
            Console.WriteLine(errorMessage);
            _dataContext.AddError(DateTime.UtcNow, errorMessage);
        }
    }
    
    private async Task HandleEventsAsync()
    {
        var chatStates = _dataContext.GetAllChatStates();
        
        foreach (var chatState in chatStates)
        {
            await TryHandleChatEventsAsync(chatState);
        }
    }

    private async Task TryHandleChatEventsAsync(ChatState chatState)
    {
        try
        {
            await HandleChatEventsAsync(chatState);
        }
        catch (Exception e)
        {
            var errorMessage = $"An error occured while handling chat \"{chatState.Name}\" event: {e.Message}";
            Console.WriteLine(errorMessage);
            _dataContext.AddError(DateTime.UtcNow, errorMessage);
        }
    }
    
    private async Task HandleChatEventsAsync(ChatState chatState)
    {
        var chat = await _telegramBotClient.GetChatAsync(chatState.ChatId);
        
        if (chat.Username != null && chat.Username.Contains(chatState.Name, StringComparison.OrdinalIgnoreCase)
            || chat.Title != null && chat.Title.Contains(chatState.Name, StringComparison.OrdinalIgnoreCase))
        {
            foreach (var chatEvent in _chatEvents)
            {
                if (chatEvent.ShouldLaunch(chatState))
                {
                    await chatEvent.HandleEventAsync(_telegramBotClient, chatState);
                    await _dataContext.SaveChangesAsync();
                }
            }
        }
    }
}
