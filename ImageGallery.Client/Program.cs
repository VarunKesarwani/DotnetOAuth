using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Net.Http.Headers;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddJsonOptions(configure =>
        configure.JsonSerializerOptions.PropertyNamingPolicy = null);


// This will clear middleware mapping of claim and will show original claim names defined in IDP
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear(); 


// Here we need two things part to take care of OpenId Connect flow and Some place to store User Identity
// create an HttpClient used for accessing the API
builder.Services.AddHttpClient("APIClient", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ImageGalleryAPIRoot"]);
    client.DefaultRequestHeaders.Clear();
    client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
});

builder.Services.AddAuthentication(o =>
    {
        // If we are hosting different application on same server we need to make sure this is unique to an application
        o.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        o.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
    }).AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme,
    o =>
    {
        o.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        o.Authority = "https://localhost:5001";
        o.ClientId = "imagegalleryclient";
        o.ClientSecret = "secret";
        o.ResponseType = "code";
        //o.Scope.Add("openid"); // These scope are called by default those we have commented it out
        //o.Scope.Add("profile");
        //o.CallbackPath = new PathString("signin-oidc"); // This is again default value
        //o.SignedOutCallbackPath = new PathString("signout-callback-oidc"); // IDP redirects to this url post sign out This is again an default value. And this needs to be registered at IDP level
        o.SaveTokens = true; // This will save cookie in application property and will be visible to User Object
        o.GetClaimsFromUserInfoEndpoint = true; // This will call UserInfo endpoint to get user identity claim information
        o.ClaimActions.Remove("aud");
        o.ClaimActions.DeleteClaim("sid");
        o.ClaimActions.DeleteClaim("idp");
        o.Scope.Add("roles");
        //o.ClaimActions.MapUniqueJsonKey("role","role"); // This is used when user can have only one role. in case user has many roles we use mapping as below.
        o.ClaimActions.MapJsonKey("role", "role");
        o.TokenValidationParameters = new()
        {
            NameClaimType = "given_name",
            RoleClaimType = "role"
        };
    });
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler();
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();// This should be registered first.
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Gallery}/{action=Index}/{id?}");

app.Run();
