using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Playground.Domain.Services;
using Playground.Infrastructure.Extensions.Akka;
using Playground.Infrastructure.Extensions.Swagger;
using Playground.Infrastructure.Repository.Extensions;
using Playground.Infrastructure.Repository.InMemory;
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
        /// <param name="services">The <see cref="IServiceCollection"/> of services to add to.</param>
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddCodeFirstGrpc();
            //services.AddConsulServiceDiscovery(_configuration);
            services.AddAkka("ServiceHost", actorBuilder =>
            {
                actorBuilder.AddFSharpActor<ContractManagerService>(ContractManagerActor.CreateActor);
            });
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
        /// <param name="app"></param>
        /// <param name="env"></param>
        public static void Configure(IApplicationBuilder app, IHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Playground v1");
                c.RoutePrefix = string.Empty;
            });

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<ContractManagerService>();
                endpoints.MapControllers();
            });

            //app.UseConsul();

            app.UseAkka();
        }
    }
}
