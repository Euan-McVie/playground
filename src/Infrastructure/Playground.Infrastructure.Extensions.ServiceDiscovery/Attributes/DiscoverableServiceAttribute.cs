using System;
using System.Collections.Generic;

namespace Playground.Infrastructure.Extensions.ServiceDiscovery.Attributes
{
    /// <summary>
    /// The <see langword="abstract"/> <see cref="DiscoverableServiceAttribute"/> is used as a base for more specific Discoverable Service attributes.
    /// This attribute is used to register the decorated service with the remote service discovery mechanism.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public abstract class DiscoverableServiceAttribute : Attribute
    {
        /// <summary>
        /// The name of the service to register with service discovery.
        /// </summary>
        public string ServiceName { get; }
        /// <summary>
        /// The type of the service to register with service discovery.
        /// </summary>
        public string ServiceType { get; }
        /// <summary>
        /// Additional tags used to group the service.
        /// </summary>
        public IEnumerable<string> Tags { get; }

        /// <summary>
        /// Contructor to set the required properties of a discoverable service.
        /// </summary>
        /// <param name="serviceName">The name of the service to register with service discovery.</param>
        /// <param name="serviceType">The type of the service to register with service discovery.</param>
        /// <param name="tags">Additional tags used to group the service.</param>
        public DiscoverableServiceAttribute(string serviceName, string serviceType, params string[] tags)
        {
            ServiceName = serviceName;
            ServiceType = serviceType;
            Tags = tags;
        }
    }
}
