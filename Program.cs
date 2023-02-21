using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

//builder.Logging.AddDebug();

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder => builder
        .WithOrigins("http://localhost:4200")
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials());
});

// Add services to the container.
builder.Services.AddSignalR();
builder.Services.AddSingleton<DashboardHub>();
builder.Services.AddSingleton<IScreenOneManager,ScreenOneManager>();
builder.Services.AddSingleton<IScreenTwoManager,ScreenTwoManager>();
builder.Services.AddSingleton<IScreenThreeManager,ScreenThreeManager>();
builder.Services.AddSingleton<ITelemetryManager,TelemetryManager>();
builder.Services.AddHostedService<DashboardDataService>();
builder.Services.AddControllers();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseCors("CorsPolicy");
app.UseRouting();


app.MapHub<DashboardHub>("/dashboard");

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

