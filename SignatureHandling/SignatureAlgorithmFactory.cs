namespace SignatureHandling;

using SignatureHandling.Interfaces;
using AmazonSecretsManagerHandler;

public static class SignatureAlgorithmFactory
{
    public static ISignatureAlgorithm Create()
    {
        return SecretsProvider.GetAlgorithmString() switch
        {
            "ecdsa" => new EcdsaSignatureAlgorithm(SecretsProvider.GetSecretKey(), SecretsProvider.GetApiKeyName()),
            "ed25519" => new Ed25519SignatureAlgorithm(SecretsProvider.GetSecretKey(), SecretsProvider.GetApiKeyName()),
            _ => throw new NotSupportedException($"Unsupported algorithm: {SecretsProvider.GetAlgorithmString()}")
        };
    }
}