namespace CoinbaseWebSocketConstructs;

using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using Jose;
using Newtonsoft.Json;
using AmazonSecretsManagerHandler;

public class CoinbaseWebSocketSubscribeMessage
{
    [JsonProperty(PropertyName = "type")]
    public string Type { get; set; }

    [JsonProperty(PropertyName = "channel")]
    public string Channel { get; set; }

    [JsonProperty(PropertyName = "product_ids")]
    public string[] ProductIds { get; set; }
    
    [JsonProperty(PropertyName = "jwt")]
    public string Jwt { get; private set; }
    


    public CoinbaseWebSocketSubscribeMessage(string type, string channel, string[] product_ids)
    {
        Type = type;
        Channel = channel;
        ProductIds = product_ids;
        Jwt = GenerateToken();
    }

    static string GenerateToken()
    {
        var privateKeyBytes = Convert.FromBase64String(ParseKey()); // Assuming PEM is base64 encoded
        using var key = ECDsa.Create();
        key.ImportECPrivateKey(privateKeyBytes, out _);

        var payload = new Dictionary<string, object>
        {
            { "sub", SecretsProvider.GetApiKeyName()},
            { "iss", "coinbase-cloud" },
            { "nbf", Convert.ToInt64((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds) },
            { "exp", Convert.ToInt64((DateTime.UtcNow.AddMinutes(1) - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds) },
        };

        var extraHeaders = new Dictionary<string, object>
        {
            { "kid", SecretsProvider.GetApiKeyName() },
            // add nonce to prevent replay attacks with a random 10 digit number
            { "nonce", RandomHex(10) },
            { "typ", "JWT"}
        };

        var encodedToken = JWT.Encode(payload, key, JwsAlgorithm.ES256, extraHeaders);
        return encodedToken;
    }

    public static bool IsTokenValid(string token, string tokenId, string secret) {
        if (token == null)
            return false;

        var key = ECDsa.Create();
        key?.ImportECPrivateKey(Convert.FromBase64String(secret), out _);

        var securityKey = new ECDsaSecurityKey(key) { KeyId = tokenId };

        try {
            var tokenHandler = new JwtSecurityTokenHandler();
            tokenHandler.ValidateToken(token, new TokenValidationParameters {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = securityKey,
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out var validatedToken);

            return true;
        } catch {
            return false;
        }
    }

    static string ParseKey() 
    {
        List<string> keyLines = new List<string>();
        keyLines.AddRange(SecretsProvider.GetSecretKey().Split('\n', StringSplitOptions.RemoveEmptyEntries));
        keyLines.RemoveAt(0);
        keyLines.RemoveAt(keyLines.Count - 1);
        return String.Join("", keyLines);
    }


    static string RandomHex(int digits) {
        using(var random = RandomNumberGenerator.Create()){
            byte[] buffer = new byte[digits / 2];
            random.GetBytes(buffer);
            string result = String.Concat(buffer.Select(x => x.ToString("X2")).ToArray());
            if (digits % 2 == 0)
                return result;
            return result + RandomNumberGenerator.GetInt32(16).ToString("X");
        }
    }
}