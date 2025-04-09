namespace AmazonSecretsManagerHandler;
using Amazon;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using AmazonSecretsManagerHandler.Models;

public static class SecretsProvider
{
    private static CoinbaseApiKey? _coinbaseCredentials;
    private static readonly object _lock = new object();
    private static bool _initialized = false;

    public static async Task Initialize()
    {
        if (!_initialized)
        {
            var credentials = await FetchCoinbaseCredentials();
            lock (_lock)
            {
                _coinbaseCredentials = credentials;
                _initialized = true;
            }
        }
    }

    public static CoinbaseApiKey GetCoinbaseCredentials()
    {
        if (!_initialized)
        {
            throw new InvalidOperationException("SecretsProvider has not been initialized. Call Initialize() first.");
        }
        
        return _coinbaseCredentials!;
    }

    private static async Task<CoinbaseApiKey> FetchCoinbaseCredentials()
    {
        string secretName = "Coinbase-Test-Key-1";
        string region = "us-east-1";
        IAmazonSecretsManager client = new AmazonSecretsManagerClient(RegionEndpoint.GetBySystemName(region));
        GetSecretValueRequest request = new GetSecretValueRequest
        {
            SecretId = secretName,
            VersionStage = "AWSCURRENT"
        };
        
        try
        {
            var response = await client.GetSecretValueAsync(request);
            string secretJson = response.SecretString;
            
            var result = JsonSerializer.Deserialize<CoinbaseApiKey>(secretJson);
            if (result == null)
            {
                throw new InvalidOperationException("Failed to deserialize Coinbase credentials");
            }

            return result;
        }
        catch (Exception e)
        {
            Console.Error.WriteLine($"Error retrieving Coinbase credentials: {e.Message}");
            throw;
        }
    }

    public static string GetApiKeyName()
    {
        if (!_initialized)
        {
            throw new InvalidOperationException("SecretsProvider has not been initialized. Call Initialize() first.");
        }

        if(_coinbaseCredentials == null){
            throw new InvalidOperationException("Failed to deserialize Coinbase credentials");
        }

        return _coinbaseCredentials.Name;
    }

    public static string GetSecretKey()
    {
        if (!_initialized)
        {
            throw new InvalidOperationException("SecretsProvider has not been initialized. Call Initialize() first.");
        }

        if(_coinbaseCredentials == null){
            throw new InvalidOperationException("Failed to deserialize Coinbase credentials");
        }

        return _coinbaseCredentials.PrivateKey;
    }
}