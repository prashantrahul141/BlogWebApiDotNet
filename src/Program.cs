using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using BlogWebApiDotNet;
using BlogWebApiDotNet.Managers;
using BlogWebApiDotNet.Models;
using BlogWebApiDotNet.Utils;
using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;

// Loading and validating environment variables.
LoadDotEnv EnvLoader = new(".env");
string[] Keys = ["PG_PASSWORD", "PG_HOST", "PG_DATABASE", "PG_USERNAME", "PG_PORT", "JWT_ISSUER", "JWT_AUD", "JWT_KEY"];
EnvLoader.ValidateEnv(ref Keys);

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => {
    options.AddSecurityDefinition("oauth2", securityScheme: new OpenApiSecurityScheme {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });

    options.OperationFilter<SecurityRequirementsOperationFilter>();
});


string PG_CONNECTION_STRING = $"Host={EnvLoader.GetKeyOrThrow("PG_HOST")};Database={EnvLoader.GetKeyOrThrow("PG_DATABASE")};Username={EnvLoader.GetKeyOrThrow("PG_USERNAME")};Password={EnvLoader.GetKeyOrThrow("PG_PASSWORD")};Include Error Detail=True";
builder.Services.AddDbContext<DataContext>(options => options.UseNpgsql(PG_CONNECTION_STRING));

builder.Services.AddIdentityCore<User>().AddEntityFrameworkStores<DataContext>().AddApiEndpoints();

builder.Services.AddAuthentication().AddBearerToken(IdentityConstants.BearerScheme);
builder.Services.AddAuthorizationBuilder();

builder.Services.AddScoped<IBlogManager, BlogManager>();
builder.Services.AddScoped<IUserManager, UserManager>();


var app = builder.Build();

if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapIdentityApi<User>();

app.UseHttpsRedirection();

app.MapControllers();


app.Run();
