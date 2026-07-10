using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PlanejadorCompras.API.Extensions;
using PlanejadorCompras.API.Security;
using PlanejadorCompras.Application;
using PlanejadorCompras.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
const string FrontendCorsPolicy = "Frontend";

var allowedCorsOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .GetChildren()
    .Select(origin => origin.Value)
    .Where(origin => !string.IsNullOrWhiteSpace(origin))
    .Select(origin => origin!)
    .ToArray();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddApiProblemDetails();
builder.Services.AddSingleton<IAuthCookieService, AuthCookieService>();
builder.Services.AddCors(options =>
{
    options.AddPolicy(FrontendCorsPolicy, policy =>
    {
        if (allowedCorsOrigins.Length == 0)
        {
            return;
        }

        policy.WithOrigins(allowedCorsOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var issuer = GetRequiredConfigurationValue(builder.Configuration, "Authentication:Jwt:Issuer");
        var audience = GetRequiredConfigurationValue(builder.Configuration, "Authentication:Jwt:Audience");
        var secretKey = GetRequiredConfigurationValue(builder.Configuration, "Authentication:Jwt:SecretKey");

        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                context.Token = context.Request.Cookies[AuthenticationConstants.AccessTokenCookieName];
                return Task.CompletedTask;
            }
        };
    });
builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseApiExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.Use(async (context, next) =>
{
    context.Response.Headers.TryAdd("X-Content-Type-Options", "nosniff");
    context.Response.Headers.TryAdd("X-Frame-Options", "DENY");
    context.Response.Headers.TryAdd("Referrer-Policy", "no-referrer");
    context.Response.Headers.TryAdd("Permissions-Policy", "camera=(), microphone=(), geolocation=()");

    if (context.Request.Path.StartsWithSegments("/api/auth"))
    {
        context.Response.Headers["Cache-Control"] = "no-store";
        context.Response.Headers["Pragma"] = "no-cache";
    }

    await next();
});
app.UseCors(FrontendCorsPolicy);
app.Use(async (context, next) =>
{
    var hasAccessTokenCookie = context.Request.Cookies.ContainsKey(AuthenticationConstants.AccessTokenCookieName);

    if (hasAccessTokenCookie
        && IsUnsafeHttpMethod(context.Request.Method)
        && !HasXmlHttpRequestHeader(context))
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        context.Response.ContentType = "application/problem+json";

        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status401Unauthorized,
            Title = "Missing required request header.",
            Instance = context.Request.Path
        };

        problemDetails.Extensions["errorCode"] = "missing_x_requested_with";

        await context.Response.WriteAsJsonAsync(problemDetails);
        return;
    }

    await next();
});
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

string GetRequiredConfigurationValue(IConfiguration configuration, string key)
{
    var value = configuration[key];

    if (string.IsNullOrWhiteSpace(value))
    {
        throw new InvalidOperationException($"Missing configuration '{key}'.");
    }

    return value;
}

bool IsUnsafeHttpMethod(string method)
{
    return HttpMethods.IsPost(method)
        || HttpMethods.IsPut(method)
        || HttpMethods.IsPatch(method)
        || HttpMethods.IsDelete(method);
}

bool HasXmlHttpRequestHeader(HttpContext context)
{
    return string.Equals(
        context.Request.Headers[AuthenticationConstants.XmlHttpRequestHeaderName],
        AuthenticationConstants.XmlHttpRequestHeaderValue,
        StringComparison.OrdinalIgnoreCase);
}
