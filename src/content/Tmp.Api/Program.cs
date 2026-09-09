using Tmp.Api;
using Tmp.Api.MiddleWare;
using Tmp.Model;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Register dependence
builder.Services.RegistService();
builder.Services.AllowCORS();
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
