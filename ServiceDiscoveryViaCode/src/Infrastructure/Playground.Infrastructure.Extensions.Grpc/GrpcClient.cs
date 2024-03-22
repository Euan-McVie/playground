using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Grpc.Net.Client;
using Playground.Infrastructure.Extensions.ServiceDiscovery.Attributes;
using Playground.Infrastructure.Extensions.ServiceDiscovery.Interfaces;
using ProtoBuf.Grpc.Client;

namespace Playground.Infrastructure.Extensions.Grpc
{
    /// <summary>
    /// Defines a client wrapper for calling a code first gRPC service.
    /// </summary>
    /// <typeparam name="TService">The code first gRPC service definition.</typeparam>
    public class GrpcClient<TService>
        where TService : class
    {
        private readonly IServiceLookup _serviceLookup;
        private readonly string _serviceName;

        /// <summary>
        /// Constructor for a gRPC client wrapper calling the code first gRPC <typeparamref name="TService"/>.
        /// </summary>
        /// <param name="serviceLookup">The service discovery <see cref="IServiceLookup"/> to use to find the service.</param>
        /// <exception cref="ArgumentException">When the <typeparamref name="TService"/> does not have the <see cref="DiscoverableGrpcServiceAttribute"/> applied
        /// or has multiple <see cref="DiscoverableGrpcServiceAttribute"/> applied within its inheritance hierarchy.</exception>
        public GrpcClient(IServiceLookup serviceLookup)
        {
            _serviceLookup = serviceLookup;

            Type serviceType = typeof(TService);
            IEnumerable<DiscoverableGrpcServiceAttribute> att = serviceType.GetCustomAttributes<DiscoverableGrpcServiceAttribute>(true);
            _serviceName = (att.Count()) switch
            {
                0 => throw new ArgumentException($"The provided service {serviceType.FullName} is not discoverable. Add the 'DiscoverableGrpcService' attribute to the service interface."),
                1 => att.First().ServiceName,
                _ => throw new ArgumentException($"The provided service {serviceType.FullName} has multiple 'DiscoverableGrpcService' attributes. Please ensure there is only one attribute applied to the service interface."),
            };
        }

        /// <summary>
        /// Makes a call to an operation on the remote gRPC service.
        /// </summary>
        /// <typeparam name="TResult">The type of result returned by the operation.</typeparam>
        /// <param name="operation">A function defining the operation on the remote <typeparamref name="TService"/> to call.</param>
        /// <returns>The result of calling the remote <paramref name="operation"/>.</returns>
        /// <exception cref="InvalidOperationException">If the address for the service cannot be found using the service discovery's <see cref="IServiceLookup"/>.</exception>
        public async Task<TResult> CallAsync<TResult>(Func<TService, Task<TResult>> operation)
        {
            if (operation is null)
                throw new ArgumentNullException(nameof(operation));

            using var channel = GrpcChannel.ForAddress(await GetServiceUriAsync().ConfigureAwait(false));
            TService client = channel.CreateGrpcService<TService>();
            return await operation(client).ConfigureAwait(false);
        }

        /// <summary>
        /// Looks up the address for the service using the service discovery's <see cref="IServiceLookup"/>.
        /// </summary>
        /// <returns>The <see cref="Uri"/> for the service.</returns>
        /// <exception cref="InvalidOperationException">If the address for the service cannot be found using the service discovery's <see cref="IServiceLookup"/>.</exception>
        private async Task<Uri> GetServiceUriAsync()
        {
            (bool found, Uri location) = await _serviceLookup.TryGetServiceLocationAsync(_serviceName, "gRPC").ConfigureAwait(false);
            if (!found)
                throw new InvalidOperationException($"Unable to find an address for the '{_serviceName}' gRPC service.");
            return location;
        }
    }
}
