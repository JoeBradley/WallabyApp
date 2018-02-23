using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Test;

namespace ITI.IdenitityServer
{
    public class Config
    {
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("User.Read", "Read user accounts"),
                new ApiResource("User.Write", "Modify user accounts"),
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            var client_id = "client";
            var client_secret = "secret";

            return new List<Client>
            {
                new Client
                {
                    ClientId = client_id,

                    // no interactive user, use the clientid/secret for authentication
                    AllowedGrantTypes = GrantTypes.ClientCredentials,

                    // secret for authentication
                    ClientSecrets =
                    {
                        new Secret(client_secret.Sha256())
                    },

                    // scopes that client has access to
                    AllowedScopes = GetApiResources().Select(x => x.Name).ToList()
                },
                new Client
                {
                    ClientId = "ro.client",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,

                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes = GetApiResources().Select(x => x.Name).ToList()
                }
            };
        }



        public static List<TestUser> GetUsers()
        {
            return new List<TestUser>
            {
                new TestUser
                {
                    SubjectId = "1",
                    Username = "alice",
                    Password = "password"
                },
                new TestUser
                {
                    SubjectId = "2",
                    Username = "bob",
                    Password = "password"
                }
            };
        }
    }
}
