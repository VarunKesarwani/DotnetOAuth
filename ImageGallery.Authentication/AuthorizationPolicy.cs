using Microsoft.AspNetCore.Authorization;

namespace ImageGallery.Authentication
{
    public static class AuthorizationPolicies
    {
        // Scope defined here is at the level of user. Those Client uses this scope.
        public static AuthorizationPolicy CanAddImage()
        {
            return new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .RequireClaim("country", "us")
                .RequireRole("PaidUser")
                .Build();

        }
    }
}
