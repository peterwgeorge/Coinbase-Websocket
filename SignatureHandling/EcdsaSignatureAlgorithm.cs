namespace SignatureHandling;

using System.Security.Cryptography;
using Jose;
using SignatureHandling.Interfaces;

public class EcdsaSignatureAlgorithm : ISignatureAlgorithm
{
    private readonly ECDsa _ecdsa;
    private readonly string _keyId;

    public EcdsaSignatureAlgorithm(string base64PemSecret, string keyId)
    {
        _ecdsa = ECDsa.Create();
        _ecdsa.ImportECPrivateKey(Convert.FromBase64String(base64PemSecret), out _);
        _keyId = keyId;
    }

    public string SignJwt(Dictionary<string, object> payload, Dictionary<string, object> extraHeaders)
    {
        extraHeaders["kid"] = _keyId;
        extraHeaders["typ"] = "JWT";
        return Jose.JWT.Encode(payload, _ecdsa, JwsAlgorithm.ES256, extraHeaders);
    }
}
