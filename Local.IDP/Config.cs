using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace Everi.IDP;

public static class Config
{
    // Varun:Set of Identity-related resources, which is mapping to set of user claims related to User
    // Varun: This deals with user scope and claims only
    public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[]
        {
            new IdentityResources.OpenId(), // Maps to user's identifier, This is required. By this subjectId of TestUsers can be requested
            new IdentityResources.Profile(), // Maps to user's name, family name, DOB, etc
            new IdentityResource("roles","Your role(s)", new []{"role"})
        };

    // Varun:This Maps to API, which APIs client application (Not Users) can access
    // Varun: This deals with API scope and claims only
    public static IEnumerable<ApiScope> ApiScopes =>
        new ApiScope[]
        {

        };

    // Varun:Those Client application are defined or configured here
    public static IEnumerable<Client> Clients =>
        // Varun: List of clients which will request for token
        new Client[]
        {
            new Client()
            {
                ClientName = "Image Gallery",
                ClientId = "imagegalleryclient",
                AllowedGrantTypes = GrantTypes.Code, // this is code flow
                RedirectUris =
                {
                    "https://localhost:7184/signin-oidc" 
                    // This is client URI where token will be redirected to,
                    // meaning uri on which this client is allowed to receive token on.
                    // signin-oidc is default uri used by middleware.
                },
                PostLogoutRedirectUris =
                {
                    "https://localhost:7184/signout-callback-oidc"
                },
                AllowedScopes =
                {
                    // This defines scopes which this client can request for
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    "roles"
                },
                ClientSecrets =
                {
                    new Secret("secret".Sha256())
                },
                RequireConsent = true // This will ask user for consent.
            }
        };
}