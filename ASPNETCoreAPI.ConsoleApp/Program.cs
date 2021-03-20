using IdentityModel.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ASPNETCoreAPI.ConsoleApp
{
    class Program
    {
        private static async Task Main(string[] args)
        {
            var client = new HttpClient();

            var discoveryDocument = await client.GetDiscoveryDocumentAsync("https://localhost:44351/");

            if (discoveryDocument.IsError)
            {
                Console.WriteLine(discoveryDocument.Error);
                return;
            }

            var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = discoveryDocument.TokenEndpoint,
                ClientId = "client",
                ClientSecret = "secret",
                Scope = "aspnetcore-api"
            });

            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error); return;
            }

            Console.WriteLine($"Token: {tokenResponse.AccessToken}");

            // call api
            var apiClient = new HttpClient();
            apiClient.SetBearerToken(tokenResponse.AccessToken);

            var response = await apiClient.GetAsync("https://localhost:44307/products");
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.StatusCode);
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine(JArray.Parse(content));
            }
        }
    }
}
