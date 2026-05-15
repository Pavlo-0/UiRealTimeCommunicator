using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthenticatedAccessTokenFactoryExample.Backend.Communication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using UiRtc.Public;

var builder = WebApplication.CreateBuilder(args);

const string jwtIssuer = "UiRtc.AuthenticatedAccessTokenFactoryExample";
const string jwtAudience = "UiRtc.AuthenticatedAccessTokenFactoryExample.Frontend";
const string jwtSigningKey = "UiRtc.AuthenticatedAccessTokenFactoryExample.SigningKey.For.Local.Demo.Only";

var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSigningKey));
var demoUsers = new Dictionary<string, DemoUser>(StringComparer.OrdinalIgnoreCase)
{
    ["demo"] = new("demo-user", "demo", "Demo User", "demo"),
    ["demo1"] = new("demo-user-1", "demo1", "Demo User 1", "demo1"),
    ["demo2"] = new("demo-user-2", "demo2", "Demo User 2", "demo2")
};

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials()
              .SetIsOriginAllowed(_ => true);
    });
});

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,
            ValidateAudience = true,
            ValidAudience = jwtAudience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = signingKey,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1),
            NameClaimType = ClaimTypes.Name
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;

                if (!string.IsNullOrWhiteSpace(accessToken) && IsUiRtcHubPath(path))
                {
                    context.Token = accessToken;
                }

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddUiRealTimeCommunicator();

var app = builder.Build();

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapPost("/auth/login", (LoginRequest request) =>
{
    if (!demoUsers.TryGetValue(request.UserName, out var user) ||
        user.Password != request.Password)
    {
        return Results.Unauthorized();
    }

    return Results.Ok(new LoginResponse(
        CreateAccessToken(user),
        user.UserId,
        user.DisplayName));
});

app.MapGet("/auth/me", (ClaimsPrincipal user) =>
{
    if (user.Identity?.IsAuthenticated != true)
    {
        return Results.Unauthorized();
    }

    return Results.Ok(new
    {
        userId = user.FindFirstValue(ClaimTypes.NameIdentifier),
        userName = user.FindFirstValue(ClaimTypes.Name)
    });
}).RequireAuthorization();

app.UseUiRealTimeCommunicator();

app.Run();

bool IsUiRtcHubPath(PathString path)
{
    return path.StartsWithSegments($"/{UiRtcHubNames.Chat}");
}

string CreateAccessToken(DemoUser user)
{
    var claims = new[]
    {
        new Claim(ClaimTypes.NameIdentifier, user.UserId),
        new Claim(ClaimTypes.Name, user.DisplayName),
        new Claim("demo_login", user.Login)
    };

    var token = new JwtSecurityToken(
        issuer: jwtIssuer,
        audience: jwtAudience,
        claims: claims,
        expires: DateTime.UtcNow.AddMinutes(30),
        signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256));

    return new JwtSecurityTokenHandler().WriteToken(token);
}

public sealed record LoginRequest(string UserName, string Password);

public sealed record LoginResponse(string AccessToken, string UserId, string UserName);

public sealed record DemoUser(string UserId, string Login, string DisplayName, string Password);
