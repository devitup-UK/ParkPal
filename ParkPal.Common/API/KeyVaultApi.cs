using ParkPal.Common.API.Models.KeyVaultApi;

namespace ParkPal.Common.API;

public class KeyVaultApi: BaseApi
{
    public KeyVaultApi(string baseUrl) : base(baseUrl)
    {
    }

    public List<Key> GetAllKeys(string environment, string token)
    {
        // TODO - Get the token from the AppSettings to.
        SetHeader("x-client-token", token);
        return GetRequest<List<Key>>("/KeyVault/GetAllEnvironmentKeys?environment=" + environment);
    }
}