using ParkPal.Common.API.Models.KeyVaultApi;

namespace ParkPal.Common.API;

public class KeyVaultApi: BaseApi
{
    public KeyVaultApi(string baseUrl) : base(baseUrl)
    {
    }

    public List<Key> GetAllKeys(string environment, string token)
    {
        SetHeader("x-client-token", token);
        List<Key>? returnedKeys = GetRequest<List<Key>>("/KeyVault/GetAllEnvironmentKeys?environment=" + environment);
        
        if (returnedKeys != null)
        {
            return returnedKeys;
        }

        return new List<Key>();
    }
}