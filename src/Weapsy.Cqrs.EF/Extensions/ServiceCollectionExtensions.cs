﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Weapsy.Cqrs.EF.Configuration;

// ReSharper disable InconsistentNaming

namespace Weapsy.Cqrs.EF.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddWeapsyCqrsEF(this IServiceCollection services, IConfiguration configuration)
        {
            services.Scan(s => s
                .FromAssembliesOf(typeof(DomainDbContext))
                .AddClasses()
                .AsImplementedInterfaces());

            services.Configure<DomainDbConfiguration>(configuration.GetSection(Constants.DomainDbConfiguration));

            return services;
        }
    }
}
