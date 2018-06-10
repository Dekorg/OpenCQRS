﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Weapsy.Cqrs.EF.Extensions;

namespace Weapsy.Cqrs.EF.SqlServer
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddWeapsyCqrsSqlServerEventStore(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddWeapsyCqrsEF(configuration);

            var connectionString = configuration.GetSection(Constants.DomainDbConfigurationConnectionString).Value;

            services.AddDbContext<DomainDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddTransient<IDatabaseProvider, SqlServerDatabaseProvider>();

            return services;
        }
    }
}
