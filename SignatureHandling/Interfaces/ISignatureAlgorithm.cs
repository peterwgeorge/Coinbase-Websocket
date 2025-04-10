namespace SignatureHandling.Interfaces;

public interface ISignatureAlgorithm
{
    string SignJwt(Dictionary<string, object> payload, Dictionary<string, object> extraHeaders);
}
