using itpe3200.DAL;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(options =>
{
    // Prevents circular reference exceptions (Group -> Members -> Group ...)
    options.JsonSerializerOptions.ReferenceHandler =
        System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});

builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ICourseRepository, CourseRepository>();

// CORS so the React app (localhost:5173) can call the API
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
        policy.SetIsOriginAllowed(origin => new Uri(origin).Host == "localhost")
              .AllowAnyMethod().AllowAnyHeader().AllowCredentials());
});

// Unexpected exceptions are returned as a ProblemDetails 500 response instead of crashing
builder.Services.AddProblemDetails();

// Log to console and to a new file per run in Logs/ (same setup as Subapp 1)
builder.Services.AddSerilog((services, loggerConfiguration) =>
{
    loggerConfiguration
        .MinimumLevel.Information()
        .WriteTo.Console()
        .WriteTo.File($"Logs/api_{DateTime.Now:yyyyMMdd_HHmmss}.log")
        // Hide noisy SQL logs from Entity Framework
        .Filter.ByExcluding(e => e.Properties.TryGetValue("SourceContext", out var value) &&
                                 e.Level == LogEventLevel.Information &&
                                 e.MessageTemplate.Text.Contains("Executed DbCommand"));
});

var app = builder.Build();

app.UseExceptionHandler();

// Applies the migrations and adds the test data
DbInit.Seed(app);

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseCors("CorsPolicy");

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
