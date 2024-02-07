using System.Text.Json;
using Amazon.S3;
using Amazon.S3.Model;

namespace MakimaBot.Model;

public class BucketClient
{
    private readonly AmazonS3Client _client;
    private readonly string _bucketName;
    private readonly string _stateFileName;

    public BucketClient(AmazonS3Client client, string bucketName, string stateFileName)
    {
        _client = client;
        _bucketName = bucketName;
        _stateFileName = stateFileName;
    }

    public async Task<BotState?> TryLoadStateAsync()
    {
        try
        {
            return await LoadStateAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occured while loading state: {ex.Message}.");
            return null;
        }
    }

    public async Task<BotState> LoadStateAsync()
    {
        var request = new GetObjectRequest
        {
            BucketName = _bucketName,
            Key = _stateFileName
        };

        using var response = await _client.GetObjectAsync(request);
        await using var responseStream = response.ResponseStream;
        using var reader = new StreamReader(responseStream);

        var state = await reader.ReadToEndAsync();
        return JsonSerializer.Deserialize<BotState>(state);
    }

    public async Task<bool> TryUpdateState(BotState state)
    {
        try
        {
            await UpdateState(state);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occured while updating state: {ex.Message}.");
            return false;
        }
    }

    private async Task UpdateState(BotState state)
    {
        var content = JsonSerializer.Serialize(state);
        
        var request = new PutObjectRequest
        {
            BucketName = _bucketName,
            Key = _stateFileName,
            ContentBody = content
        };

        await _client.PutObjectAsync(request);
    }
}
