using Microsoft.Extensions.DependencyInjection;
using Playground.Infrastructure.Repository.Interfaces;

namespace Playground.Infrastructure.Repository.Extensions
{
    /// <summary>
    /// Extension methods to manage <see cref="IRepository"/> implementations.
    /// </summary>
    public static class RepositoryExtensions
    {
        /// <summary>
        /// Add the provided <typeparamref name="TRepository"/> to the <see cref="IServiceCollection"/>
        /// bound to the <see cref="IRepository"/> interface.
        /// </summary>
        /// <typeparam name="TRepository">The type of repository to add.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the repository to.</param>
        /// <returns>The updated <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddRepository<TRepository>(
            this IServiceCollection services)
            where TRepository : class, IRepository
        {
            services.AddSingleton<IRepository, TRepository>();
            return services;
        }
    }
}
