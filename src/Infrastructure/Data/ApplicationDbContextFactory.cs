using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace CleanArchitecture.Infrastructure.Data;
public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        // Build configuration to read appsettings.json
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();

        // Get connection string (use the same name as in your DependencyInjection.cs)
        var connectionString = configuration.GetConnectionString("CleanArchitectureDb")
            ?? throw new InvalidOperationException("Connection string 'CleanArchitectureDb' not found.");

        // Create DbContext options
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

        // Configure based on your database provider
        // Check your DependencyInjection.cs to see which one you're using

#if UsePostgreSQL
            optionsBuilder.UseNpgsql(connectionString);
#elif UseSqlite
            optionsBuilder.UseSqlite(connectionString);
#else
        // Default: SQL Server
        optionsBuilder.UseSqlServer(connectionString);
#endif

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
