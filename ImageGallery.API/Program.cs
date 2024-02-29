using System.IdentityModel.Tokens.Jwt;
using ImageGallery.API.DbContexts;
using ImageGallery.API.Services;
using ImageGallery.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddControllers()
    .AddJsonOptions(configure => configure.JsonSerializerOptions.PropertyNamingPolicy = null);

builder.Services.AddDbContext<GalleryContext>(options =>
{
    options.UseSqlite(
        builder.Configuration["ConnectionStrings:ImageGalleryDBConnectionString"]);
});

// register the repository
builder.Services.AddScoped<IGalleryRepository, GalleryRepository>();

// register AutoMapper-related services
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

// Add services to the container.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddOAuth2Introspection(o => {
        //o.Authority = "https://localhost:6001";
        o.Authority = "https://localhost:44300";
        o.ClientId = "imagegalleryapi";
        o.ClientSecret = "apiscecret";
        o.NameClaimType = "given_name";
        o.RoleClaimType = "role";
    });
    //.AddJwtBearer(o =>
    //{
    //    o.Authority = "https://localhost:6001"; // this is used for loading Metadata from IDP (well-known doc)
    //    o.Audience = "imagegalleryapi";
    //    // This is added to avoid Jwt Confusion attack. So that API only validate api defined here.
    //    o.TokenValidationParameters = new TokenValidationParameters()
    //    {
    //        NameClaimType = "given_name",
    //        RoleClaimType = "role",
    //        ValidTypes = new[] { "at+jwt" }
    //    };
    //});

builder.Services.AddAuthorization(option =>
{
    option.AddPolicy("UserCanAddImage", AuthorizationPolicies.CanAddImage());
    option.AddPolicy("CanAddImage", policyBuild =>
    {
        policyBuild.RequireClaim("scope", "imagegalleryapi.write");
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
