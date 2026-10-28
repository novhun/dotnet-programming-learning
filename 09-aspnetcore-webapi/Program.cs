using AspNetCoreWebApi.Endpoints;
using AspNetCoreWebApi.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to DI container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails(); // RFC 7807 standardized error formatting

var app = builder.Build();

// Configure Middleware Pipeline
app.UseRequestTiming(); // Custom timing middleware

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Global root welcome endpoint
app.MapGet("/", () => Results.Ok(new
{
    Message = "Welcome to the ASP.NET Core Course Web API!",
    Docs = "/openapi/v1.json",
    Endpoints = "/api/courses"
}));

// Map modular Course Endpoints
app.MapCourseEndpoints();

Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine("=========================================================");
Console.WriteLine("   PART 09: ASP.NET CORE WEB API RUNNING");
Console.WriteLine("=========================================================");
Console.ResetColor();

app.Run();
