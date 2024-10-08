﻿using Telegram.Bot;

namespace MakimaBot.Model.Events;

public class ChatEventsHandler
{
    private readonly ITelegramBotClientWrapper _telegramBotClientWrapper;
    private readonly IEnumerable<IChatEvent> _chatEvents;
    private readonly IDataContext _dataContext;

    public ChatEventsHandler(
        ITelegramBotClientWrapper telegramBotClientWrapper,
        IEnumerable<IChatEvent> chatEvents,
        IDataContext dataContext)
    {
        _telegramBotClientWrapper = telegramBotClientWrapper;
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
            _dataContext.AddOrUpdateError(DateTime.UtcNow, errorMessage);
            await _dataContext.SaveChangesAsync();
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
            _dataContext.AddOrUpdateError(DateTime.UtcNow, errorMessage);
            await _dataContext.SaveChangesAsync();
        }
    }
    
    private async Task HandleChatEventsAsync(ChatState chatState)
    {
        var chat = await _telegramBotClientWrapper.GetChatAsync(chatState.ChatId);
        
        if (chat.Username != null && chat.Username.Contains(chatState.Name, StringComparison.OrdinalIgnoreCase)
            || chat.Title != null && chat.Title.Contains(chatState.Name, StringComparison.OrdinalIgnoreCase))
        {
            foreach (var chatEvent in _chatEvents)
            {
                if (chatEvent.ShouldLaunch(chatState))
                {
                    await chatEvent.HandleEventAsync(_telegramBotClientWrapper, chatState);
                    await _dataContext.SaveChangesAsync();
                }
            }
        }
    }
}
