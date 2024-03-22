using Microsoft.Extensions.Hosting;
using Playground.Infrastructure.Extensions.Console;

namespace Playground.TestClient
{
    class MainProgram
    {
        static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureConsoleDefaults(consoleBuilder =>
                {
                    consoleBuilder.UseStartup<Startup>();
                });
    }
}
