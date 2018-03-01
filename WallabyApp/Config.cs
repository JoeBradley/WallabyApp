using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Test;
using IdentityServer4;
using System.Security.Claims;

namespace ITI.IdenitityServer
{
    public class Config
    {
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("Identity", "API Identity"),
                new ApiResource("User", "User accounts") {
                    Scopes =
                    {
                        new Scope()
                        {
                            Name = "user.full_access",
                            DisplayName = "Full read/write access to User API"
                        },
                        new Scope
                        {
                            Name = "user.read_only",
                            DisplayName = "Read only access to User API"
                        }
                    }
                },
                new ApiResource("Products", "Company Products"),
            };
        }

        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            var client_id = "client";
            var client_secret = "secret";

            return new List<Client>
            {
                // API Client (Wallaby Console Client)
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

                // Resource Owner client
                new Client
                {
                    ClientId = "ro.client",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,

                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes = GetApiResources().Where(x => x.Name == "Identity" || x.Name == "User").SelectMany(x => x.Scopes.Select(s => s.Name)).ToList()
                },

                // MVC Client (Wallaby Web Client)
                new Client
                {
                    ClientId = "mvc",
                    ClientName = "MVC Client",
                    AllowedGrantTypes = GrantTypes.Implicit,

                    // where to redirect to after login
                    RedirectUris = { "http://localhost:5002/signin-oidc" },

                    // where to redirect to after logout
                    PostLogoutRedirectUris = { "http://localhost:5002/signout-callback-oidc" },

                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile
                    }
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
                    Password = "password",

                    Claims = new []
                    {
                        new Claim("name", "Alice"),
                        new Claim("website", "https://alice.com"),
                        new Claim("position", "King of the world"),
                    }
                },
                new TestUser
                {
                    SubjectId = "2",
                    Username = "bob",
                    Password = "password",

                    Claims = new []
                    {
                        new Claim("name", "Bob"),
                        new Claim("website", "https://bob.com")
                    }
                }
            };
        }
    }
}
