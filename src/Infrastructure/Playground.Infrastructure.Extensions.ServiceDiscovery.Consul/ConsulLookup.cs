using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Consul;
using Playground.Infrastructure.Extensions.ServiceDiscovery.Interfaces;

namespace Playground.Infrastructure.Services.ServiceDiscovery.Consul
{
    internal class ConsulLookup : IServiceLookup
    {
        private readonly IConsulClient _consulClient;

        internal ConsulLookup(IConsulClient consulClient)
        {
            _consulClient = consulClient;
        }

        /// <inheritdoc/>
        public async Task<(bool IsFound, Uri Location)> TryGetServiceLocationAsync(string serviceName, string serviceType)
        {
            QueryResult<Dictionary<string, AgentService>> services = await _consulClient.Agent.Services().ConfigureAwait(false);
            if (services.Response.TryGetValue($"{serviceName}-{serviceType}", out AgentService? service))
                return (true, new Uri($"{service.Address}:{service.Port}"));
            return (false, new Uri(string.Empty));
        }
    }
}
