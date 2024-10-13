using System.Text.Json;
using Amazon.S3;
using Amazon.S3.Model;

namespace MakimaBot.Model;

public class BucketClient : IStateClient
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

    public async Task<string> LoadRawStateAsync()
    {
        var request = new GetObjectRequest
        {
            BucketName = _bucketName,
            Key = _stateFileName
        };

        using var response = await _client.GetObjectAsync(request);
        await using var responseStream = response.ResponseStream;
        using var reader = new StreamReader(responseStream);

        return await reader.ReadToEndAsync();
    }

    public async Task<BotState> LoadStateAsync()
    {
        var state = await LoadRawStateAsync();
        return JsonSerializer.Deserialize<BotState>(state);
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
            Console.WriteLine($"An error occured while updating state: {ex.Message}.");
            return false;
        }
    }

    private async Task UpdateStateAsync(BotState state)
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
