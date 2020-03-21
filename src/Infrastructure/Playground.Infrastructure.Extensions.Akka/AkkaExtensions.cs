using System;
using Akka.Actor;
using Akka.DI.Core;
using Akka.DI.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Playground.Infrastructure.Extensions.Akka
{
    /// <summary>
    /// Extension methods to aid interop with Akka.Net
    /// </summary>
    public static class AkkaExtensions
    {
        /// <summary>
        /// Add and initialise an Akka.Net <see cref="ActorSystem"/> to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add Akka.Net services to.</param>
        /// <param name="systemActorName">The name of the actor system to create.</param>
        /// <param name="akkaActorBuilder">An <see cref="Action{T}"/> to further configure the provided <see cref="IAkkaActorBuilder"/>.</param>
        /// <returns>The updated <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddAkka(
            this IServiceCollection services, string systemActorName, Action<IAkkaActorBuilder>? akkaActorBuilder = null)
        {
            services.AddSingleton(provider => ActorSystem.Create(systemActorName).UseServiceProvider(provider));

            if (akkaActorBuilder != null)
            {
                var builder = new AkkaActorBuilder(services);
                akkaActorBuilder(builder);
            }

            return services;
        }

        /// <summary>
        /// Adds and registers the Akka.Net <see cref="ActorSystem"/> to be started and stopped by the <see cref="IHostApplicationLifetime"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IApplicationBuilder"/> to add the middleware to.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IApplicationBuilder UseAkka(
            this IApplicationBuilder builder)
        {
            if (builder is null)
                throw new ArgumentNullException(nameof(builder));

            IHostApplicationLifetime lifetime = builder.ApplicationServices.GetService<IHostApplicationLifetime>();

            // Setup logger
            ILoggerFactory loggingFactory = builder.ApplicationServices
                                .GetRequiredService<ILoggerFactory>();
            ILogger<IApplicationBuilder> logger = loggingFactory.CreateLogger<IApplicationBuilder>();

            lifetime.ApplicationStarted.Register(() =>
            {
                logger.LogInformation("Starting Akka actor system");
                builder.ApplicationServices.GetService<ActorSystem>(); // start Akka.NET
            });
            lifetime.ApplicationStopping.Register(() =>
            {
                logger.LogInformation("Stopping Akka actor system");
                builder.ApplicationServices.GetService<ActorSystem>().Terminate().Wait();
            });

            return builder;
        }
    }

    /// <summary>
    /// Provider method to return a <see cref="IActorRef"/> to an instance of the requested <typeparamref name="TActor"/> actor.
    /// </summary>
    /// <typeparam name="TActor">The actor type to return an <see cref="IActorRef"/> to an instance of.</typeparam>
    /// <returns>The <see cref="IActorRef"/> actor reference.</returns>
    public delegate IActorRef ActorProvider<TActor>();

    /// <summary>
    /// Defines a contract for a Akka.Net actor builder in an application.
    /// An Akka.Net actor builder provides programmatic configuration for adding Akka.Net actors to the <see cref="ActorSystem"/>
    /// and enables retrival of their <see cref="IActorRef"/> from the <see cref="IServiceProvider"/> via their <see cref="ActorProvider{TActor}"/>
    /// </summary>
    public interface IAkkaActorBuilder
    {
        /// <summary>
        /// Adds a C# style actor that can be retrieved from the <see cref="IServiceProvider"/> via their <see cref="ActorProvider{TActor}"/>
        /// </summary>
        /// <typeparam name="TActor">The type of C# actor to create.</typeparam>
        /// <returns>The <see cref="IAkkaActorBuilder"/> to continue configuring further actors on.</returns>
        IAkkaActorBuilder AddActor<TActor>() where TActor : ActorBase, new();

        /// <summary>
        /// Adds a F# style actor that can be retrieved from the <see cref="IServiceProvider"/> via their <see cref="ActorProvider{TActor}"/>
        /// </summary>
        /// <typeparam name="TActor">The type of F# actor to create.</typeparam>
        /// <param name="createActorFunc">The F# function that creates the actor.</param>
        /// <returns>The <see cref="IAkkaActorBuilder"/> to continue configuring further actors on.</returns>
        /// <example>
        /// The F# creation function is expected to spawn the the actor as a child of the <see cref="IActorRefFactory"/> parent actor
        /// that will be provided to it.
        /// <code>
        /// module MyActor =
        ///     let private myActor (mailbox: Actor&lt;obj&gt;) =
        ///         ...
        /// 
        ///     let CreateActor parentActor = 
        ///         spawn parentActor
        ///         &lt;| "MyActorName"
        ///         &lt;| myActor
        /// </code>
        /// </example>
        IAkkaActorBuilder AddFSharpActor<TActor>(Func<IActorRefFactory, IActorRef> createActorFunc);

        /// <summary>
        /// Adds a F# style actor that can be retrieved from the <see cref="IServiceProvider"/> via their <see cref="ActorProvider{TActor}"/>
        /// </summary>
        /// <typeparam name="TActor">The type of F# actor to create.</typeparam>
        /// <param name="createActorFunc">The F# function that creates the actor.</param>
        /// <returns>The <see cref="IAkkaActorBuilder"/> to continue configuring further actors on.</returns>
        /// <example>
        /// The F# creation function is expected to spawn the the actor as a child of the <see cref="IActorRefFactory"/> parent actor
        /// that will be provided to it.
        /// <code>
        /// module MyActor =
        ///     let private myActor serviceProvider (mailbox: Actor&lt;obj&gt;) =
        ///         ...
        /// 
        ///     let CreateActor parentActor serviceProvider = 
        ///         spawn parentActor
        ///         &lt;| "MyActorName"
        ///         &lt;| myActor serviceProvider
        /// </code>
        /// </example>
        IAkkaActorBuilder AddFSharpActor<TActor>(Func<IActorRefFactory, IServiceProvider, IActorRef> createActorFunc);
    }

    internal class AkkaActorBuilder : IAkkaActorBuilder
    {
        private readonly IServiceCollection _services;

        internal AkkaActorBuilder(IServiceCollection services)
        {
            _services = services;
        }

        public IAkkaActorBuilder AddActor<TActor>()
            where TActor : ActorBase, new()
        {
            _services.AddSingleton<ActorProvider<TActor>>(provider =>
            {
                ActorSystem actorSystem = provider.GetService<ActorSystem>();
                IActorRef actorRef = actorSystem.ActorOf(actorSystem.DI().Props<TActor>());
                return () => actorRef;
            });

            return this;
        }

        public IAkkaActorBuilder AddFSharpActor<TActor>(Func<IActorRefFactory, IActorRef> createActorFunc)
        {
            _services.AddSingleton<ActorProvider<TActor>>(provider =>
            {
                ActorSystem actorSystem = provider.GetService<ActorSystem>();
                IActorRef actorRef = createActorFunc(actorSystem);
                return () => actorRef;
            });
            return this;
        }

        public IAkkaActorBuilder AddFSharpActor<TActor>(Func<IActorRefFactory, IServiceProvider, IActorRef> createActorFunc)
        {
            _services.AddSingleton<ActorProvider<TActor>>(provider =>
            {
                ActorSystem actorSystem = provider.GetService<ActorSystem>();
                IActorRef actorRef = createActorFunc(actorSystem, provider);
                return () => actorRef;
            });
            return this;
        }
    }
}
