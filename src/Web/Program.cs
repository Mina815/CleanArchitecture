using System.Reflection;
using CleanArchitecture.Infrastructure.Data;
using Scalar.AspNetCore;

static bool IsOpenApiDocumentGeneration() =>
    AppDomain.CurrentDomain.GetAssemblies()
        .Any(a => a.GetName().Name?.Contains("GetDocument", StringComparison.OrdinalIgnoreCase) == true);

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddServiceDefaults();

builder.AddKeyVaultIfConfigured();
builder.AddApplicationServices();
builder.AddInfrastructureServices();
builder.AddWebServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!IsOpenApiDocumentGeneration())
{
    await app.MigrateDatabaseAsync();

    if (app.Environment.IsDevelopment())
        await app.SeedDatabaseAsync();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseCors(static builder => 
    builder.AllowAnyMethod()
        .AllowAnyHeader()
        .AllowAnyOrigin());

app.UseAuthentication();
app.UseAuthorization();

app.UseFileServer();

app.MapOpenApi();
app.MapScalarApiReference();

app.UseExceptionHandler(options => { });

#if (UseApiOnly)
app.Map("/", () => Results.Redirect("/scalar"));
#endif

app.MapDefaultEndpoints();
app.MapHub<CleanArchitecture.Web.Hubs.BookingHub>("/hubs/bookings");
app.MapEndpoints(typeof(Program).Assembly);

#if (!UseApiOnly)
app.MapFallbackToFile("index.html");
#endif

app.Run();
