using CleanArchitecture.Shared;

var builder = DistributedApplication.CreateBuilder(args);

builder.AddAzureContainerAppEnvironment("aca-env");

#if (UsePostgreSQL)
var dbUsername = builder.AddParameter("postgres-username", "postgres");
var dbPassword = builder.AddParameter("postgres-password", "postgres", secret: true);

var postgres = builder.AddPostgres(Services.DatabaseServer, dbUsername, dbPassword)
    .WithImage("postgres", "15")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume();

var databaseServer = postgres.AddDatabase(Services.Database);
#elif (UseSqlServer)
var databaseServer = builder
    .AddAzureSqlServer(Services.DatabaseServer)
    .RunAsContainer(container => 
        container.WithLifetime(ContainerLifetime.Persistent))
    .AddDatabase(Services.Database);
#else
var databaseServer = builder
    .AddSqlite(Services.Database);
#endif

var web = builder.AddProject<Projects.Web>(Services.WebApi)
    .WithReference(databaseServer)
    .WaitFor(databaseServer)
    .WithExternalHttpEndpoints()
    .WithAspNetCoreEnvironment()
    .WithUrlForEndpoint("http", url =>
    {
        url.DisplayText = "Scalar API Reference";
        url.Url = "/scalar";
    });

#if (!UseApiOnly)
if (builder.ExecutionContext.IsRunMode)
{
    builder.AddJavaScriptApp(Services.WebFrontend, "./../Web/ClientApp")
        .WithRunScript("start")
        .WithReference(web)
        .WaitFor(web)
        .WithHttpEndpoint(env: "PORT")
        .WithExternalHttpEndpoints();
}
#endif

builder.Build().Run();
