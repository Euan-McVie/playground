using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Playground.Infrastructure.Extensions.ServiceDiscovery.Attributes;
using Playground.Infrastructure.Extensions.ServiceDiscovery.Interfaces;
using Playground.Infrastructure.Services.ServiceDiscovery.Consul;

namespace Playground.Infrastructure.Services.ServiceDiscovery
{
    /// <summary>
    /// Extension methods to enable Consul as the service discovery provider.
    /// </summary>
    public static class ConsulExtensions
    {
        /// <summary>
        /// Adds Consul service discovery to the <see cref="IServiceCollection"/>. 
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add Consul service discovery to.</param>
        /// <returns>The updated <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddConsulServiceDiscovery(
            this IServiceCollection services)
        {
            return services
                .AddSingleton<IConsulClient, ConsulClient>(provider => new ConsulClient(consulConfig =>
                {
                    IConfiguration configuration = provider.GetService<IConfiguration>();
                    string address = configuration["Consul:Address"];
                    consulConfig.Address = new Uri(address);
                }))
                .AddSingleton<IServiceLookup, ConsulLookup>(sp => new ConsulLookup(sp.GetService<IConsulClient>()));
        }

        /// <summary>
        /// Registers services with a <see cref="DiscoverableServiceAttribute"/> to Consul using the <see cref="IHostApplicationLifetime"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IApplicationBuilder"/> to add the middleware to.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IApplicationBuilder UseConsul(
            this IApplicationBuilder builder)
        {
            if (builder is null)
                throw new ArgumentNullException(nameof(builder));

            // Retrieve Consul client from DI
            IConsulClient consulClient = builder.ApplicationServices
                                .GetRequiredService<IConsulClient>();

            // Retrieve lifetime from DI
            IHostApplicationLifetime lifetime = builder.ApplicationServices
                                .GetRequiredService<IHostApplicationLifetime>();

            // Setup logger
            ILoggerFactory loggingFactory = builder.ApplicationServices
                                .GetRequiredService<ILoggerFactory>();
            ILogger<IApplicationBuilder> logger = loggingFactory.CreateLogger<IApplicationBuilder>();

            lifetime.ApplicationStarted.Register(() => RegisterServices(builder, lifetime, consulClient, logger));

            return builder;
        }

        private static void RegisterServices(
            IApplicationBuilder builder,
            IHostApplicationLifetime lifetime,
            IConsulClient consulClient,
            ILogger<IApplicationBuilder> logger)
        {
            var registrations = new List<AgentServiceRegistration>();

            // Get discoverable services
            IEnumerable<DiscoverableServiceAttribute?> serviceDetails = AppDomain.CurrentDomain.GetAssemblies()
                .Where(ass => !ass.GlobalAssemblyCache)
                .SelectMany(ass => ass
                    .GetTypes()
                    .SelectMany(type => type.GetCustomAttributes<DiscoverableServiceAttribute>())
                    .Where(ds => ds != null));

            // Get server IP address
            if (builder.Properties["server.Features"] is FeatureCollection features)
            {
                IServerAddressesFeature addresses = features.Get<IServerAddressesFeature>();
                foreach (string address in addresses.Addresses)
                {
                    var uri = new Uri(address);
                    string hostAddress = $"{uri.Scheme}://{uri.Host}";
                    foreach (DiscoverableServiceAttribute? service in serviceDetails)
                    {
                        // Register service with consul
                        string serviceId = $"{service!.ServiceName}-{service!.ServiceType}";
                        logger.LogDebug($"Registering service '{serviceId}' with Consul using address {hostAddress}:{uri.Port}");
                        var registration = new AgentServiceRegistration()
                        {
                            ID = serviceId,
                            Name = service!.ServiceName,
                            Address = hostAddress,
                            Port = uri.Port,
                            Tags = service.Tags.Prepend(service!.ServiceType).ToArray()
                        };
                        registrations.Add(registration);
                        consulClient.Agent.ServiceDeregister(registration.ID).Wait();
                        consulClient.Agent.ServiceRegister(registration).Wait();
                    }
                }
            }

            if (registrations.Any())
            {
                logger.LogInformation($"Registered {registrations.Count} services with Consul");


                lifetime.ApplicationStopping.Register(() =>
                {
                    foreach (AgentServiceRegistration registration in registrations)
                        consulClient.Agent.ServiceDeregister(registration.ID).Wait();
                    logger.LogInformation("Unregistered from Consul");
                });
            }
        }
    }
}
