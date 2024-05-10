using Assignment.Api;
using Assignment.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Assignment.Api.Middleware;

using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Swagger;
using System.Reflection;
using Assignment.Api.Controllers;
using Assignment.Service.Services;
using Assignment.Api.Interfaces;
using Assignment.Infrastructure.AuditLog;
using Assignment.Api.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var envFilePath = builder.Configuration["EnvFilePath"];
DotNetEnv.Env.Load(envFilePath);

var connectionString = Environment.GetEnvironmentVariable("ConnectionString");
//var connectionString = builder.Configuration.GetConnectionString("ConnectionString");

builder.Services.AddDbContext<RaidenDBContext>(options =>
{
    options.UseSqlServer(connectionString);
    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

},ServiceLifetime.Transient);
builder.Services.AddDbContext<RaceViewContext>(options =>
{
    options.UseSqlServer(connectionString);
    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

}, ServiceLifetime.Transient);
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

//Added the Jwt token configuration. 
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = Environment.GetEnvironmentVariable("ValidIssuer"),       
        ValidAudience = Environment.GetEnvironmentVariable("ValidAudience"),
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("Secret")))
        //IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]))

    };
    options.Events = new JwtBearerEvents
    {
        OnForbidden = context =>
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "application/json";
            var errorMessage = "You don't have permission to access this resource.";
            var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new { message = errorMessage }));
            return context.Response.Body.WriteAsync(bytes, 0, bytes.Length);
        }
       
    };
});

//Configuring Swagger to take Bearer token as input.
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Auth API", Version = "v1" });
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    option.IncludeXmlComments(xmlPath);
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token and Bearer keyword",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });

    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement() {
            {
                new OpenApiSecurityScheme {
                    Reference = new OpenApiReference {
                        Type = ReferenceType.SecurityScheme,
                        Id = "oauth2"
                    },
                    Scheme = "oauth2",
                    Name = "authorization",
                    In = ParameterLocation.Header
                },
                new List <string> ()
            } });
    option.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            Implicit = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri($"https://accounts.google.com/o/oauth2/auth"),
                TokenUrl = new Uri($"https://oauth2.googleapis.com/token"),
                Scopes = new Dictionary<string, string>
                    {
                        {
                            $"https://www.googleapis.com/auth/userinfo.email",
                            "Get email"
                        }
                    }
            }
        }
    }
    );

    option.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
    option.IgnoreObsoleteActions();
    option.IgnoreObsoleteProperties();
    option.CustomSchemaIds(type => type.FullName);
});
builder.Services.AddControllers().AddJsonOptions(x => {
    x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

var services = builder.Services;
var configuration = builder.Configuration;
DI.ConfigureServices(services, configuration);

var app = builder.Build();
// Configure the HTTP request pipeline.


    app.UseSwagger();
    app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();

//Using Custom Middleware to handleexceition globally.
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

app.MapControllers();
app.Run();
