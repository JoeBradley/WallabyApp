using IdentityModel.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace WallabyClient
{
    public class Program
    {
        public static void Main(string[] args) => MainAsync().GetAwaiter().GetResult();

        private static async Task MainAsync()
        {
            // discover endpoints from metadata
            var disco = await DiscoveryClient.GetAsync("http://localhost:5000");
            if (disco.IsError)
            {
                Console.WriteLine(disco.Error);
                return;
            }

            // await ClientCredentialsGrant(disco);

            await ResourceOwnerGrant(disco);            
        }

        private static async Task ClientCredentialsGrant(DiscoveryResponse disco)
        {
            var client_id = "client";
            var client_secret = "secret";
            var scope = "User.Read User.Write";

            // request token
            var tokenClient = new TokenClient(disco.TokenEndpoint, client_id, client_secret);
            var tokenResponse = await tokenClient.RequestClientCredentialsAsync(scope);

            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                return;
            }

            Console.WriteLine(tokenResponse.Json);
            Console.WriteLine("\n\n");

            // call api
            var client = new HttpClient();
            client.SetBearerToken(tokenResponse.AccessToken);

            var response = await client.GetAsync("http://localhost:5001/identity");
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

        private static async Task ResourceOwnerGrant(DiscoveryResponse disco)
        {
            var client_id = "ro.client";
            var client_secret = "secret";
            var scope = "User.Read User.Write";
            var username = "alice";
            var password = "password";

            // request token
            var tokenClient = new TokenClient(disco.TokenEndpoint, client_id, client_secret);
            var tokenResponse = await tokenClient.RequestResourceOwnerPasswordAsync(username, password, scope);

            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                return;
            }

            Console.WriteLine(tokenResponse.Json);
            Console.WriteLine("\n\n");
        } 
    }
}
