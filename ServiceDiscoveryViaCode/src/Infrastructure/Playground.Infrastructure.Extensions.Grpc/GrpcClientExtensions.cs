using Microsoft.Extensions.DependencyInjection;

namespace Playground.Infrastructure.Extensions.Grpc
{
    /// <summary>
    /// Extension methods to construct a gRPC client.
    /// </summary>
    public static class GrpcClientExtensions
    {
        /// <summary>
        /// Add a gRPC client for the provided <typeparamref name="TService"/>.
        /// The service's location will be found using the configured service discovery.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the gRPC client to.</param>
        /// <returns>The updated <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddGrpcClient<TService>(this IServiceCollection services)
            where TService : class
        {
            services.AddScoped<GrpcClient<TService>>();
            return services;
        }
    }
}
