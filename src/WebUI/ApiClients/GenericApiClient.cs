using System.Net.Http;
using System.Runtime.Intrinsics.X86;
using System;

namespace WebUI.ApiClients;

public static class HttpClientExtensions
{
    public static async Task<TResponse?> PostJsonAsync<TRequest, TResponse>(
        this HttpClient httpClient,
        string url,
        TRequest requestBody,
        ILogger? logger = null)
    {
        try
        {
            var response = await httpClient.PostAsJsonAsync(url, requestBody);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<TResponse>();
            return result;
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Error occurred while calling {Url}", url);
            return default;
        }
    }
}