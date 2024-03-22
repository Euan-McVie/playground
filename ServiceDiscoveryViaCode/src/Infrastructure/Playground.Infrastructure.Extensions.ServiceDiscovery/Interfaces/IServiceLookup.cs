using System;
using System.Threading.Tasks;

namespace Playground.Infrastructure.Extensions.ServiceDiscovery.Interfaces
{
    /// <summary>
    /// Contract for looking up a service location from a discovery service.
    /// </summary>
    public interface IServiceLookup
    {
        /// <summary>
        /// Try to find the location for the provided <paramref name="serviceName"/>.
        /// </summary>
        /// <param name="serviceName">The name of the service to find.</param>
        /// <param name="serviceType">The type of the service to find.</param>
        /// <returns>A <see cref="ValueTuple"/> containing an <c>IsFound</c> flag and the <c>Location</c> <see cref="Uri"/> if it is.</returns>
        Task<(bool IsFound, Uri Location)> TryGetServiceLocationAsync(string serviceName, string serviceType);
    }
}
