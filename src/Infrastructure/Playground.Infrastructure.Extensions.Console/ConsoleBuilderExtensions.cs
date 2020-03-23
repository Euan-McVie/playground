using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Playground.Infrastructure.Extensions.Console
{
    /// <summary>
    /// Extension methods to enable <c>Startup.cs</c> behaviour for console applications.
    /// </summary>
    public static class ConsoleBuilderExtensions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IConsoleBuilder"/> class with pre-configured defaults.
        /// </summary>
        /// <param name="builder">The <see cref="IHostBuilder"/> instance to configure.</param>
        /// <param name="configure">The configure callback.</param>
        /// <returns>The <see cref="IHostBuilder"/> for chaining.</returns>
        public static IHostBuilder ConfigureConsoleDefaults(this IHostBuilder builder, Action<IConsoleBuilder> configure)
        {
            if (configure is null)
                throw new ArgumentNullException(nameof(configure));
            var consuleBuilder = new ConsoleBuilder(builder);
            configure(consuleBuilder);
            consuleBuilder.Build();
            return builder;
        }
    }

    /// <summary>
    /// A builder for a console <see cref="IHost"/>.
    /// </summary>
    public interface IConsoleBuilder
    {
        /// <summary>
        /// Specify the startup type to be used by the console.
        /// </summary>
        /// <typeparam name="TStartup">The type containing the startup methods for the application.</typeparam>
        /// <returns>The <see cref="IConsoleBuilder"/>.</returns>
        IConsoleBuilder UseStartup<TStartup>() where TStartup : class;
    }

    /// <summary>
    /// Defines a class that provides the mechanisms to configure a console application.
    /// </summary>
    public interface IConsoleApplicationBuilder
    {
        /// <summary>
        /// Gets the <see cref="IServiceProvider"/> that provides access to the application's service container.
        /// </summary>
        IServiceProvider ApplicationServices { get; }
    }

    internal class ConsoleApplicationBuilder : IConsoleApplicationBuilder
    {
        public ConsoleApplicationBuilder(IServiceProvider provider)
        {
            ApplicationServices = provider;
        }

        public IServiceProvider ApplicationServices { get; }
    }

    internal class ConsoleApplicationHostedService : IHostedService
    {
        private readonly object _startup;
        private readonly MethodInfo _configureMethod;
        private readonly IServiceProvider _serviceProvider;

        public ConsoleApplicationHostedService(object startup, MethodInfo configureMethod, IServiceProvider serviceProvider)
        {
            _startup = startup;
            _configureMethod = configureMethod;
            _serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            // Create a scope for Configure, this allows creating scoped dependencies
            // without the hassle of manually creating a scope.
            using (IServiceScope scope = _serviceProvider.CreateScope())
            {
                IServiceProvider serviceProvider = scope.ServiceProvider;
                ParameterInfo[] parameterInfos = _configureMethod.GetParameters();
                object[] parameters = new object[parameterInfos.Length];
                for (int index = 0; index < parameterInfos.Length; index++)
                {
                    ParameterInfo parameterInfo = parameterInfos[index];
                    if (parameterInfo.ParameterType == typeof(IConsoleApplicationBuilder))
                    {
                        parameters[index] = new ConsoleApplicationBuilder(serviceProvider);
                    }
                    else
                    {
                        try
                        {
                            parameters[index] = serviceProvider.GetRequiredService(parameterInfo.ParameterType);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(
                                $"Could not resolve a service of type '{parameterInfo.ParameterType.FullName}' for the parameter '{parameterInfo.Name}' of " +
                                $"method '{_configureMethod.Name}' on type '{_configureMethod.DeclaringType!.FullName}'.",
                                ex);
                        }
                    }
                }
                _configureMethod.Invoke(_startup, parameters);
            }
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }

    internal class ConsoleBuilder : IConsoleBuilder
    {
        private readonly IHostBuilder _builder;
        private Type? _startupType;

        public ConsoleBuilder(IHostBuilder builder)
        {
            _builder = builder;
        }

        public void Build()
        {
            if (_startupType != null)
            {
                object? startup = Activator.CreateInstance(_startupType);
                if (startup != null)
                {
                    MethodInfo? configureServicesMethod = _startupType.GetMethod("ConfigureServices");
                    if (configureServicesMethod != null)
                        _builder.ConfigureServices((context, services) => configureServicesMethod.Invoke(startup, new[] { services }));

                    MethodInfo? configureMethod = _startupType.GetMethod("Configure");
                    if (configureMethod != null)
                        _builder.ConfigureServices((context, services) => services.AddHostedService(provider => new ConsoleApplicationHostedService(startup, configureMethod, provider)));
                }
            }
        }

        /// <inheritdoc/>
        public IConsoleBuilder UseStartup<TStartup>()
            where TStartup : class
        {
            _startupType = typeof(TStartup);
            return this;
        }
    }
}
