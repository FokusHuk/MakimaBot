using System.Text.Encodings.Web;
using System.Text.Json;

namespace MakimaBot.Model;

public class LocalStorageStateProvider(string pathToLocalStateFile) : IStateProvider
{
    public async Task<string> LoadRawStateAsync()
    {
        return await File.ReadAllTextAsync(pathToLocalStateFile);
    }

    public async Task<BotState> LoadStateAsync()
    {
        var rawState = await LoadRawStateAsync();
        return JsonSerializer.Deserialize<BotState>(rawState);
    }

    public async Task<bool> TryUpdateStateAsync(BotState state)
    {
        try
        {
            await UpdateStateAsync(state);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occured while updating local state: {ex.Message}.");
            return false;
        }
    }

    private async Task UpdateStateAsync(BotState state)
    {
        var content = JsonSerializer.Serialize(
            state, 
            new JsonSerializerOptions 
            {   
                WriteIndented = true, 
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping 
            }
        );

        await File.WriteAllTextAsync(pathToLocalStateFile, content);
    }
}
