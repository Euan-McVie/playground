using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Playground.Domains.ContractManagement.Services;
using Playground.Infrastructure.Extensions.Akka;
using Playground.Infrastructure.Extensions.Swagger;
using Playground.Infrastructure.Repository.Extensions;
using Playground.Infrastructure.Repository.InMemory;
using Playground.Infrastructure.Services.ServiceDiscovery;
using Playground.Services.ContractManagement;
using Playground.Services.ContractManagement.Actors;
using ProtoBuf.Grpc.Server;

namespace Playground.ServiceHost
{
    /// <summary>
    /// Definition of the startup routine for the service host.
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
            services.AddMvc();

            services.AddCodeFirstGrpc();

            services.AddConsulServiceDiscovery();

            services.AddAkka("ServiceHost", actorBuilder =>
            {
                // Add top level actors that should be accessible from DI.
                actorBuilder.AddFSharpActor<ContractManagerService>(ContractManagerActor.CreateActor);
            });

            //Add services
            services.AddScoped<IContractManager, ContractManagerService>();

            services.AddRepository<InMemoryDictionaryRepository>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Playground", Version = "v1" });
                c.IncludeAllXmlComments(true);
            });
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/> to add the middleware to.</param>
        /// <param name="env">The <see cref="IHostEnvironment"/> providing hosting environment information.</param>
        public static void Configure(IApplicationBuilder app, IHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Playground v1");
                c.RoutePrefix = string.Empty;
            });

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                //Enable gRPC services
                endpoints.MapGrpcService<ContractManagerService>();

                //Enable WepAPI controllers
                endpoints.MapControllers();
            });

            app.UseConsul();

            app.UseAkka();
        }
    }
}
