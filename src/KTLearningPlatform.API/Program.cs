using KTLearningPlatform.Core.Interfaces;
using KTLearningPlatform.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

builder.Services.AddScoped<IAuthService, PlatformServices>();
builder.Services.AddScoped<IJoinerService, PlatformServices>();
builder.Services.AddScoped<IKTContentService, PlatformServices>();
builder.Services.AddScoped<IQuizService, PlatformServices>();
builder.Services.AddScoped<IAdminDashboardService, PlatformServices>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
