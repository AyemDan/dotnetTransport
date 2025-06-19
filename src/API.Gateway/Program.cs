using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using API.Gateway.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configure PostgreSQL Database
builder.Services.AddDbContext<GatewayDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection") ?? 
        "Host=localhost;Port=5432;Database=TransportApp;Username=postgres;Password=root"));

// Configure JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"] ?? "YourSuperSecretKeyHereMakeItLongEnoughForSecurity";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "TransportApp";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "TransportApp";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "Transport API Gateway", 
        Version = "v1",
        Description = "API Gateway for Transport Microservices with Authentication"
    });
    
    // Add security definitions
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add HTTP client for service communication
builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Transport API Gateway v1");
        c.RoutePrefix = string.Empty; // Serve Swagger UI at root
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

// Basic routing to microservices
app.MapGet("/", () => "🚀 Transport API Gateway is running!");

// Health check endpoint
app.MapGet("/health", () => new { Status = "Healthy", Timestamp = DateTime.UtcNow });

// Service endpoints
app.MapGet("/services", () => new
{
    Services = new[]
    {
        new { Name = "Payment Service", Url = "http://localhost:5057", Status = "Running" },
        new { Name = "Notification Service", Url = "http://localhost:5264", Status = "Running" },
        new { Name = "Student App", Url = "http://localhost:5155", Status = "Running" },
        new { Name = "Provider App", Url = "http://localhost:5281", Status = "Running" },
        new { Name = "Organization App", Url = "http://localhost:5065", Status = "Running" }
    }
});

// Gateway endpoints that can route to specific services
app.MapGet("/api/payment/{*path}", async (string path, IHttpClientFactory httpClientFactory) =>
{
    var client = httpClientFactory.CreateClient();
    var response = await client.GetAsync($"http://localhost:5057/api/payment/{path}");
    return Results.Ok(new { Message = "Payment service endpoint", Path = path });
});

app.MapGet("/api/notification/{*path}", async (string path, IHttpClientFactory httpClientFactory) =>
{
    var client = httpClientFactory.CreateClient();
    var response = await client.GetAsync($"http://localhost:5264/api/notification/{path}");
    return Results.Ok(new { Message = "Notification service endpoint", Path = path });
});

app.MapGet("/api/student/{*path}", async (string path, IHttpClientFactory httpClientFactory) =>
{
    var client = httpClientFactory.CreateClient();
    var response = await client.GetAsync($"http://localhost:5155/api/student/{path}");
    return Results.Ok(new { Message = "Student app endpoint", Path = path });
});

app.MapGet("/api/provider/{*path}", async (string path, IHttpClientFactory httpClientFactory) =>
{
    var client = httpClientFactory.CreateClient();
    var response = await client.GetAsync($"http://localhost:5281/api/provider/{path}");
    return Results.Ok(new { Message = "Provider app endpoint", Path = path });
});

app.MapGet("/api/organization/{*path}", async (string path, IHttpClientFactory httpClientFactory) =>
{
    var client = httpClientFactory.CreateClient();
    var response = await client.GetAsync($"http://localhost:5065/api/organization/{path}");
    return Results.Ok(new { Message = "Organization app endpoint", Path = path });
});

app.Run(); 