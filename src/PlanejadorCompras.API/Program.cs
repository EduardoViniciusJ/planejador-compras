using PlanejadorCompras.API.Configuration;
using PlanejadorCompras.API.Extensions;
using PlanejadorCompras.API.Security;
using PlanejadorCompras.Application;
using PlanejadorCompras.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddApiProblemDetails();
builder.Services.AddSingleton<IAuthCookieService, AuthCookieService>();
builder.Services.AddApiCors(builder.Configuration);
builder.Services.AddApiAuthentication(builder.Configuration);
builder.Services.AddApiRateLimiting();
builder.Services.AddApiProxySupport(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseApiPipeline();
app.MapControllers();

app.Run();

public partial class Program;
