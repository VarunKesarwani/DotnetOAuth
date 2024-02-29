using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using Serilog;

namespace Everi.IDP;

internal static class HostingExtensions
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {

        builder.Services.Configure<IISOptions>(options =>
        {
            options.AuthenticationDisplayName = "Windows";
            options.AutomaticAuthentication = false;
        });
        builder.Services.Configure<IISServerOptions>(options =>
        {
            options.AuthenticationDisplayName = "Windows";
            options.AutomaticAuthentication = false;
        });
        // Varun:This will add razor page UI to IDP
        builder.Services.AddRazorPages();

        builder.Services.AddIdentityServer(options =>
            {
                // https://docs.duendesoftware.com/identityserver/v6/fundamentals/resources/api_scopes#authorization-based-on-scopes
                options.EmitStaticAudienceClaim = true;
            })
            .AddInMemoryIdentityResources(Config.IdentityResources)
            .AddInMemoryApiResources(Config.ApiResources)
            .AddInMemoryApiScopes(Config.ApiScopes)
            .AddInMemoryClients(Config.Clients)
            .AddTestUsers(TestUsers.Users); // Varun: This is user mapping to IDP

        builder.Services
            .AddAuthentication()
        .AddOpenIdConnect("AAD", "Azure AD", o =>
        {
            o.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
            o.Authority = "https://login.microsoftonline.com/f0df1368-0ac6-4b93-ac33-fbb9939e0769/v2.0";
            o.ClientId = "246e7a78-4677-4b6d-a9b7-c7b738baac55";
            o.ClientSecret = "Yuf8Q~5Rltyz517rx148NVxxsRwYP7jVesm5pcUp";
            o.ResponseType = "code";
            o.CallbackPath = new PathString("/signin-aad/");
            o.SignedOutCallbackPath = new PathString("/signout-aad");
            o.Scope.Add("email");
            o.Scope.Add("offline_access");
            o.SaveTokens = true;
        });


        return builder.Build();
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        app.UseSerilogRequestLogging();

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        // uncomment if you want to add a UI
        app.UseStaticFiles();
        app.UseRouting();

        app.UseIdentityServer();

        // uncomment if you want to add a UI
        app.UseAuthorization();
        app.MapRazorPages().RequireAuthorization();

        return app;
    }
}
