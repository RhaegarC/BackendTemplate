using Tmp.Api;
using Tmp.Api.MiddleWare;
using Tmp.Model;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Register dependence. Configuration is read here, at the composition root: IConfiguration
// layers environment variables over appsettings.json, so the flat keys below keep their
// existing env var names and deployments need no changes.
builder.Services.RegistService(builder.Configuration[Constant.ConfigKey.DBCon]);
builder.Services.AllowCORS(builder.Configuration);
builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseCors(Constant.App.CORSPolicyName);

app.MapControllers();

app.UseMiddleware<AuthMiddleWare>();

app.UseHealthChecks(Constant.App.HealthCheckUrl);

app.Run();
