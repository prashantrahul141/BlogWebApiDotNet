using System.Text.Json.Serialization;
using BlogWebApiDotNet;
using BlogWebApiDotNet.Managers;
using BlogWebApiDotNet.Models;
using BlogWebApiDotNet.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;

// Loading and validating environment variables.
LoadDotEnv EnvLoader = new(".env");
string[] Keys =
[
    "PG_PASSWORD",
    "PG_HOST",
    "PG_DATABASE",
    "PG_USERNAME",
    "PG_PORT",
    "JWT_ISSUER",
    "JWT_AUD",
    "JWT_KEY"
];
EnvLoader.ValidateEnv(ref Keys);

var builder = WebApplication.CreateBuilder(args);

// Ignores infinite references when serializing data to json.
builder
    .Services.AddControllers()
    .AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

// swagger stuff.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition(
        "oauth2",
        securityScheme: new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey
        }
    );

    options.OperationFilter<SecurityRequirementsOperationFilter>();
});

// setup PostgresDB instance.
string PG_CONNECTION_STRING =
    $"Host={EnvLoader.GetKeyOrThrow("PG_HOST")};Database={EnvLoader.GetKeyOrThrow("PG_DATABASE")};Username={EnvLoader.GetKeyOrThrow("PG_USERNAME")};Password={EnvLoader.GetKeyOrThrow("PG_PASSWORD")};Include Error Detail=True";
builder.Services.AddDbContext<DataContext>(options => options.UseNpgsql(PG_CONNECTION_STRING));

// .NET 8 IdentityCore using our custom AppUser.
builder
    .Services.AddIdentityCore<AppUser>()
    .AddEntityFrameworkStores<DataContext>()
    .AddApiEndpoints();

// Add auth
builder.Services.AddAuthentication().AddBearerToken(IdentityConstants.BearerScheme);
builder.Services.AddAuthorizationBuilder();

// Managers.
builder.Services.AddScoped<IBlogManager, BlogManager>();
builder.Services.AddScoped<IUserManager, AppUserManager>();

var app = builder.Build();

// when in development mode.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

/// map identity apis under /api/auth.
app.MapGroup("/api/auth").MapIdentityApi<AppUser>();

app.UseHttpsRedirection();

app.MapControllers();

// Run!!!
app.Run();
