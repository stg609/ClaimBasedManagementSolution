using System.Collections.Generic;
using IdentityServer4;
using IdentityServer4.Models;

namespace IdServer.Services
{
    public class Config
    {
        public static List<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("api1")
            };
        }

        public static List<Client> GetClients()
        {
            return new List<Client>
            {
                new Client
                {
                    // ClientId = "mvc",
                    //ClientName="Sample Business Service SPA",
                    // AllowedGrantTypes = GrantTypes.Implicit,

                    //RequireConsent = false,
                    ClientId = "mvc",
                    ClientName = "Sample MVC Client",
                    AllowedGrantTypes = GrantTypes.HybridAndClientCredentials,
                     
                    //
                    //AlwaysIncludeUserClaimsInIdToken = true,

                    RequireConsent = false,

                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },

                    // where to redirect to after login
                    RedirectUris = { "http://localhost:5002/signin-oidc" },

                    // where to redirect to after logout
                    PostLogoutRedirectUris = { "http://localhost:5002/signout-callback-oidc" },

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "api1"
                    },
                    AllowOfflineAccess = true
                }
            };
        }

        public static List<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
            };
        }
    }
}
