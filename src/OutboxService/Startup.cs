using Abstractions.Bus;
using Abstractions.DB;
using Abstractions.Serialization;
using Abstractions.Service;
using DB;
using Logic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serialization;
using Transport;

namespace OutboxService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<OutboxContext>(options => options.UseNpgsql(_configuration.GetConnectionString("DefaultConnection")));

            services.AddSingleton<IOutbox, Outbox>();
            services.AddSingleton<IOutboxSender, KafkaOutboxSender>();
            services.AddSingleton<IOutboxUnitOfWork, OutboxUnitOfWork>();
            services.AddSingleton(typeof(ISerializer<>), typeof(JsonSerializer<>));

            services.AddScoped<IOutboxMessageProcessor, OutboxMessageProcessor>();
            services.AddScoped<IOutboxUnitOfWork, OutboxUnitOfWork>();

            //TODO Kafka Producer
            //TODO KafkaOutboxSenderOptions
            //TODO OutboxHostedServiceOptions

            services.AddHealthChecks();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/hc");
            });
        }

        private readonly IConfiguration _configuration;
    }
}
