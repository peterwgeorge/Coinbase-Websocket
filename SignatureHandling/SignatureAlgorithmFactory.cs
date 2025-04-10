namespace SignatureHandling;

using SignatureHandling.Interfaces;

public static class SignatureAlgorithmFactory
{
    public static ISignatureAlgorithm Create(SigningMetadata metadata)
    {
        return metadata.Algorithm switch
        {
            "ecdsa" => new EcdsaSignatureAlgorithm(metadata.Secret, metadata.KeyId),
            "ed25519" => new Ed25519SignatureAlgorithm(metadata.Secret, metadata.KeyId),
            _ => throw new NotSupportedException($"Unsupported algorithm: {metadata.Algorithm}")
        };
    }
}