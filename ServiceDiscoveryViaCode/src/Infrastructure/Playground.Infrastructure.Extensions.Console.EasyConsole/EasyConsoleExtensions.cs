using EasyConsoleCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Playground.Infrastructure.Extensions.Console.EasyConsole
{
    /// <summary>
    /// Extension methods to use EasyConsole as the menu system for the console application.
    /// </summary>
    public static class EasyConsoleExtensions
    {
        /// <summary>
        /// Specify the Easy Console root menu <see cref="Program"/> to be used by the console application.
        /// </summary>
        /// <typeparam name="TRootMenu">The type containing the Easy Console root menu <see cref="Program"/></typeparam>
        /// <param name="app">The <see cref="IConsoleApplicationBuilder"/> to configure.</param>
        /// <returns>The <see cref="IConsoleApplicationBuilder"/>.</returns>
        public static IConsoleApplicationBuilder UseEasyConsole<TRootMenu>(this IConsoleApplicationBuilder app) where TRootMenu : Program
        {
            if (app is null)
                throw new System.ArgumentNullException(nameof(app));

            IHostApplicationLifetime lifetime = app.ApplicationServices.GetService<IHostApplicationLifetime>();

            Program console = ActivatorUtilities.CreateInstance<TRootMenu>(app.ApplicationServices);

            lifetime.ApplicationStarted.Register(() =>
            {
                console.Run();
            });

            return app;
        }
    }
}
