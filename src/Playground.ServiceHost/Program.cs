using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Playground.ServiceHost
{
    /// <summary>
    /// Main program controller.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Main entry point.
        /// </summary>
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }


        /// <summary>
        /// Create the service host builder.
        /// </summary>
        /// <returns>The <see cref="IHostBuilder"/> to run the service host.</returns>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
