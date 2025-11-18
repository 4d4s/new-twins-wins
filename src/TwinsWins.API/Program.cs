using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;
using TwinsWins.Infrastructure.Data;
using TwinsWins.Core.Interfaces;
using TwinsWins.Infrastructure.Services;
using TwinsWins.API.Hubs;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Npgsql;
using TwinsWins.Core.Enums;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/twinswins-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddSignalR();

// Create NpgsqlDataSource with enum mappings (Modern Npgsql 7.0+ approach)
var dataSourceBuilder = new NpgsqlDataSourceBuilder(builder.Configuration.GetConnectionString("DefaultConnection"));
dataSourceBuilder.MapEnum<GameStatus>("GameStatus");
dataSourceBuilder.MapEnum<GameType>("GameType");
dataSourceBuilder.MapEnum<ParticipantRole>("ParticipantRole");
dataSourceBuilder.MapEnum<BlockchainTransactionStatus>("TransactionStatus");
dataSourceBuilder.MapEnum<BlockchainTransactionType>("TransactionType");
dataSourceBuilder.MapEnum<CouponType>("CouponType");
dataSourceBuilder.MapEnum<AuditLevel>("AuditLevel");
dataSourceBuilder.MapEnum<Difficulty>("Difficulty");
var dataSource = dataSourceBuilder.Build();

// Register the data source
builder.Services.AddSingleton(dataSource);

// Database - use the data source
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(dataSource));

// Redis
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
});

// Authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.ASCII.GetBytes(jwtSettings["Secret"] ?? throw new InvalidOperationException("JWT Secret not configured"));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };

    // Allow JWT in SignalR
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;
            
            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
            {
                context.Token = accessToken;
            }
            
            return Task.CompletedTask;
        }
    };
});

// CORS (Development only - use specific origins in production)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        corsBuilder => corsBuilder
            .SetIsOriginAllowed(_ => true)  // Allow any origin in development
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});

// Services
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<IAntiCheatService, AntiCheatService>();
builder.Services.AddScoped<IBlockchainService, BlockchainService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Health Checks
builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("DefaultConnection")!)
    .AddRedis(builder.Configuration.GetConnectionString("Redis")!);

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "TwinsWins API",
        Version = "v1",
        Description = "API for TwinsWins image pair matching game"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<GameHub>("/hubs/game");

// Health check endpoints
app.MapHealthChecks("/health/live");
app.MapHealthChecks("/health/ready");

// Apply migrations on startup (only in development)
//if (app.Environment.IsDevelopment())
//{
//    using var scope = app.Services.CreateScope();
//    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
//    await dbContext.Database.MigrateAsync();
//}

app.Run();
