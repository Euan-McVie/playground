using Microsoft.Extensions.DependencyInjection;
using Playground.Domains.ContractManagement.Services;
using Playground.Infrastructure.Extensions.Console;
using Playground.Infrastructure.Extensions.Grpc;
using Playground.Infrastructure.Services.ServiceDiscovery;

namespace Playground.TestClient
{
    /// <summary>
    /// Definition of the startup routine for the console.
    /// </summary>
    public sealed class Startup
    {
        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddConsulServiceDiscovery();
            services.AddGrpcClient<IContractManager>();
        }
        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the console application.
        /// </summary>
        /// <param name="app">The <see cref="IConsoleApplicationBuilder"/> to add the middleware to.</param>
        public static void Configure(IConsoleApplicationBuilder app)
        {
            app.UseEasyConsole<RootMenu>();
        }
    }
}
