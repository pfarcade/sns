using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        string workspaceId = "<workspace_id>";
        string clientId = "<client_id>";
        string clientSecret = "<client_secret>";
        string tenantId = "<tenant_id>";
        string resourceId = "/subscriptions/{subscriptionId}/resourceGroups/{resourceGroupName}/providers/Microsoft.OperationalInsights/workspaces/{workspaceName}";

        string accessToken = await GetAccessToken(clientId, clientSecret, tenantId);

        string requestUrl = $"https://management.azure.com{resourceId}/api/logs";
        string requestBody = $"{{ \"query\": \"SecurityEvent | take 10\" }}"; 

        HttpClient client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        HttpResponseMessage response = await client.PostAsync(requestUrl, new StringContent(requestBody));
        string responseContent = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine(responseContent);
        }
        else
        {
            Console.WriteLine("An error occurred:");
            Console.WriteLine(responseContent);
        }
    }

    static async Task<string> GetAccessToken(string clientId, string clientSecret, string tenantId)
    {
        string tokenUrl = $"https://login.microsoftonline.com/{tenantId}/oauth2/token";

        using (HttpClient client = new HttpClient())
        {
            var form = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "client_credentials"),
                new KeyValuePair<string, string>("client_id", clientId),
                new KeyValuePair<string, string>("client_secret", clientSecret),
                new KeyValuePair<string, string>("resource", "https://management.azure.com/")
            });

            HttpResponseMessage response = await client.PostAsync(tokenUrl, form);
            string responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                dynamic tokenResponse = Newtonsoft.Json.JsonConvert.DeserializeObject(responseContent);
                return tokenResponse.access_token;
            }

            throw new Exception("Failed to retrieve access token.");
        }
    }
}
