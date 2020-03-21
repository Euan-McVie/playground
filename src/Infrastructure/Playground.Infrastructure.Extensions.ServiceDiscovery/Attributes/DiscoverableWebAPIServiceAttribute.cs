﻿namespace Playground.Infrastructure.Extensions.ServiceDiscovery.Attributes
{
    /// <summary>
    /// This attribute is used to register the decorated Web API service with the remote service discovery mechanism.
    /// </summary>
    public class DiscoverableWebAPIServiceAttribute : DiscoverableServiceAttribute
    {
        /// <summary>
        /// Contructor to set the required properties of a discoverable Web API service.
        /// </summary>
        /// <param name="serviceName">The name of the service to register with service discovery.</param>
        /// <param name="tags">Additional tags used to group the service.</param>
        public DiscoverableWebAPIServiceAttribute(string serviceName, params string[] tags)
            : base(serviceName, "WebAPI", tags)
        {
        }
    }
}
