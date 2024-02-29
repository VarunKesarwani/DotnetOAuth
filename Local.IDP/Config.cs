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
            new IdentityResource("roles","Your role(s)", new []{"role"}), // Adding Custom scope
            new IdentityResource("country","Country of living", new List<string> {"country"}) // Adding additional scope for ABAC
        };

    // API Resource define physical or logical API collection like productapi,orderapi, customerapi
    public static IEnumerable<ApiResource> ApiResources => new ApiResource[]
    {
        new ApiResource("imagegalleryapi", "Image Gallery API", new[] {"role","country"})
        {
            Scopes = { "imagegalleryapi.fullaccess", "imagegalleryapi.read", "imagegalleryapi.write" },
            ApiSecrets = {new Secret("apiscecret".Sha256()) }
        },
        new ApiResource("producapi"),
        new ApiResource("orderapi")
    };

    // Varun: This Maps to API, which APIs client application (Not Users) can access
    // Varun: This deals with API scope and claims only. In simple words defines api access control like imagegalleryapi.read or imagegalleryapi.write
    // Varun: This scope has nothing to do with user
    // Varun: We will check for this at API level before we allow access to that function.
    public static IEnumerable<ApiScope> ApiScopes =>
        new ApiScope[]
        {
            new ApiScope("imagegalleryapi.fullaccess"),
            new ApiScope("imagegalleryapi.read"),
            new ApiScope("imagegalleryapi.write")
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
                AccessTokenType = AccessTokenType.Reference, //AccessTokenType.Jwt for self contained token
                AllowedGrantTypes = GrantTypes.Code, // this is code flow
                AllowOfflineAccess = true,
                UpdateAccessTokenClaimsOnRefresh = true, // Lets say claims in access token for user has changed for exmaple country,
                                                         // in that case this value will not be refreshed untill access token is refreshed.
                                                         // In order to avoid this and refresh claim value this property is used.
                IdentityTokenLifetime = 300,
                AuthorizationCodeLifetime = 300,
                AccessTokenLifetime = 120, //default is 3600s or 1H
                SlidingRefreshTokenLifetime = 1200000,
                RedirectUris =
                {
                    // This is client URI where token will be redirected to,
                    // meaning uri on which this client is allowed to receive token on.
                    // signin-oidc is default uri used by middleware.
                    "https://localhost:7184/signin-oidc"
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
                    "roles",
                    //"imagegalleryapi.fullaccess", // Adding api resource to client scope.
                    "imagegalleryapi.read",
                    "imagegalleryapi.write",
                    "country"
                },
                ClientSecrets =
                {
                    new Secret("secret".Sha256())
                },
                RequireConsent = true // This will ask user for consent.
            },
            new Client()
        };
}