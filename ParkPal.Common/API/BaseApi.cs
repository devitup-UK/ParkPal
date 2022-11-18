using System.Text.Json.Serialization;
using Newtonsoft.Json;
using ParkPal.Common.API.Enums;
using RestSharp;

namespace ParkPal.Common.API;

public class BaseApi
{
    private RestClient _client;
    private Dictionary<string, string> _headers;

    public BaseApi()
    {
        _headers = new Dictionary<string, string>();
    }
    
    public BaseApi(string baseUrl)
    {
        _client = new RestClient(baseUrl);
        _headers = new Dictionary<string, string>();
    }

    public BaseApi(string baseUrl, Dictionary<string, string> headers)
    {
        _client = new RestClient(baseUrl);
        _headers = headers;
    }

    public void SetBaseUrl(string baseUrl)
    {
        _client = new RestClient(baseUrl);
    }

    public void SetHeader(string name, string value)
    {
        _headers.Add(name, value);
    }

    public void SetToken(string token)
    {
        _headers.Add("Authorization", token);
    }

    public T? GetRequest<T>(string endpoint, string? content = null) where T : class
    {
        RestRequest request = new RestRequest(endpoint, Method.Get);
        return SendRequest<T>(request, content);
    }
    
    public T? PostRequest<T>(string endpoint, string content) where T : class
    {
        RestRequest request = new RestRequest(endpoint, Method.Post);
        return SendRequest<T>(request, content);
    }
    
    public T? PutRequest<T>(string endpoint, string content) where T : class
    {
        RestRequest request = new RestRequest(endpoint, Method.Put);
        return SendRequest<T>(request, content);
    }

    private T? SendRequest<T>(RestRequest request, string? content) where T : class
    {
        request.AddHeaders(_headers);
        
        if (content != null)
        {
            request.AddParameter("application/json", content, ParameterType.RequestBody);
        }

        string? responseString = _client.Execute(request).Content;
        if (responseString != null)
        {
            return JsonConvert.DeserializeObject<T>(responseString);
        }

        return null;
    }
}