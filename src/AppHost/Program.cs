var builder = DistributedApplication.CreateBuilder(args);
const string databaseName = "CleanArchitectureDb";
const string webApiName = "webapi";
const string webFrontendName = "webfrontend";

builder.AddAzureContainerAppEnvironment("aca-env");

#if (UsePostgreSQL)
var databaseServer = builder
    .AddAzurePostgresFlexibleServer("dbserver")
    .WithPasswordAuthentication()
    .RunAsContainer(container => 
        container.WithLifetime(ContainerLifetime.Persistent))
    .AddDatabase(databaseName);
#elif (UseSqlServer)
var databaseServer = builder
    .AddAzureSqlServer("dbserver")
    .RunAsContainer(container => 
        container.WithLifetime(ContainerLifetime.Persistent))
    .AddDatabase(databaseName);
#else
var databaseServer = builder
    .AddAzureSqlServer("dbserver")
    .RunAsContainer(container => 
        container.WithLifetime(ContainerLifetime.Persistent))
    .AddDatabase(databaseName);
#endif

var web = builder.AddProject<Projects.Web>(webApiName)
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
    builder.AddJavaScriptApp(webFrontendName, "./../Web/ClientApp")
        .WithRunScript("start")
        .WithReference(web)
        .WaitFor(web)
        .WithHttpEndpoint(env: "PORT")
        .WithExternalHttpEndpoints();
}
#endif

builder.Build().Run();
