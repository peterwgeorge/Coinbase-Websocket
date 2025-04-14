namespace AmazonSecretsManagerHandler;

using Amazon;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using AmazonSecretsManagerHandler.Models;

public static class SecretsProvider
{
    private static SigningMetadata? _credentials;
    private static readonly object _lock = new object();
    private static bool _initialized = false;

    public static async Task Initialize()
    {
        if (!_initialized)
        {
            var credentials = await FetchCredentials();
            lock (_lock)
            {
                _credentials = credentials;
                _initialized = true;
            }
        }
    }

    private static async Task<SigningMetadata> FetchCredentials()
    {
        string secretName = "Coinbase-Ed25519";
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
            
            var result = JsonConvert.DeserializeObject<SigningMetadata>(secretJson);
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

    
    public static string GetAlgorithmString()
    {
        if (!_initialized)
        {
            throw new InvalidOperationException("SecretsProvider has not been initialized. Call Initialize() first.");
        }

        if(_credentials == null){
            throw new InvalidOperationException("Failed to deserialize Coinbase credentials");
        }

        return _credentials.Algorithm;
    }

    public static string GetApiKeyName()
    {
        if (!_initialized)
        {
            throw new InvalidOperationException("SecretsProvider has not been initialized. Call Initialize() first.");
        }

        if(_credentials == null){
            throw new InvalidOperationException("Failed to deserialize Coinbase credentials");
        }

        return _credentials.KeyId;
    }

    public static string GetSecretKey()
    {
        if (!_initialized)
        {
            throw new InvalidOperationException("SecretsProvider has not been initialized. Call Initialize() first.");
        }

        if(_credentials == null){
            throw new InvalidOperationException("Failed to deserialize Coinbase credentials");
        }

        return _credentials.Algorithm == "ecdsa" ? ExtractPemContent(_credentials.Secret) : _credentials.Secret;
    }

    public static string ExtractPemContent(string pem)
    {
        var lines = pem.Split('\n', StringSplitOptions.RemoveEmptyEntries)
                    .Where(line => !line.StartsWith("-----"))
                    .ToArray();
        return string.Concat(lines);
    }

}