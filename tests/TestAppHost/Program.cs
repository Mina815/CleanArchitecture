const string databaseName = "CleanArchitectureDb";
var builder = DistributedApplication.CreateBuilder(args);

#if (UsePostgreSQL)
builder.AddPostgres("dbserver")
    .AddDatabase(databaseName);
#elif (UseSqlServer)
builder.AddSqlServer("dbserver")
    .AddDatabase(databaseName);
#else
builder
    .AddSqlite(databaseName);
#endif

builder.Build().Run();
public partial class Program
{
}