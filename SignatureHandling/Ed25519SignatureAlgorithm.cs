namespace SignatureHandling;

using SignatureHandling.Interfaces;
using NSec.Cryptography;
using System;
using System.Collections.Generic;

public class Ed25519SignatureAlgorithm : ISignatureAlgorithm
{
    private readonly byte[] _privateKeySeed;
    private readonly string _keyId;

    public Ed25519SignatureAlgorithm(string base64Secret, string keyId)
    {
        byte[] fullKeyBytes = Convert.FromBase64String(base64Secret);
        if (fullKeyBytes.Length == 64)
        {
            // This is a standard Ed25519 key pair (32-byte private seed + 32-byte public key)
            // Extract just the private key seed (first 32 bytes)
            _privateKeySeed = new byte[32];
            Array.Copy(fullKeyBytes, 0, _privateKeySeed, 0, 32);
        }
        else if (fullKeyBytes.Length == 32)
        {
            // Already just the seed
            _privateKeySeed = fullKeyBytes;
        }
        else
        {
            throw new ArgumentException($"Invalid Ed25519 key format. Expected 32 or 64 bytes, got {fullKeyBytes.Length}");
        }
        
        _keyId = keyId;
    }

public string SignJwt(Dictionary<string, object> payload, Dictionary<string, object> extraHeaders)
{
    // Create a key from the seed
    var key = Key.Import(SignatureAlgorithm.Ed25519, _privateKeySeed, KeyBlobFormat.RawPrivateKey);
    extraHeaders["kid"] = _keyId;
    extraHeaders["typ"] = "JWT";
    extraHeaders["alg"] = "EdDSA"; // Ed25519 uses EdDSA algorithm identifier
    
    // Create the JWT manually
    var header = System.Text.Json.JsonSerializer.Serialize(extraHeaders);
    var payloadJson = System.Text.Json.JsonSerializer.Serialize(payload);
    var headerBase64 = Base64UrlEncode(System.Text.Encoding.UTF8.GetBytes(header));
    var payloadBase64 = Base64UrlEncode(System.Text.Encoding.UTF8.GetBytes(payloadJson));
    var signatureInput = $"{headerBase64}.{payloadBase64}";
    var signatureInputBytes = System.Text.Encoding.UTF8.GetBytes(signatureInput);
    var signature = SignatureAlgorithm.Ed25519.Sign(key, signatureInputBytes);
    return $"{signatureInput}.{Base64UrlEncode(signature)}";
}

private static string Base64UrlEncode(byte[] data)
{
    return Convert.ToBase64String(data)
        .Replace('+', '-')
        .Replace('/', '_')
        .TrimEnd('=');
}
}
