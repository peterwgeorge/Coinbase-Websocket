namespace SignatureHandling;

using SignatureHandling.Interfaces;

public class Ed25519SignatureAlgorithm : ISignatureAlgorithm {
    
    public Ed25519SignatureAlgorithm(string base64PemSecret, string keyId){
        
    }

    public string SignJwt(Dictionary<string, object> payload, Dictionary<string, object> extraHeaders)
    {
        return "";
    }

}