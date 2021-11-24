using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using Abstractions.Bus;
using Abstractions.DB;
using Abstractions.Service;
using Confluent.Kafka;
using DB;
using Logic;
using OutboxService.Config;
using Serialization;
using Transport;
using Transport.Validation;
using OutboxService.Services;
using OutboxService.Validation;

using AS = Abstractions.Serialization;


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

            services.AddScoped<IOutbox, Outbox>();
            services.AddScoped<IOutboxFetcher, OutboxFetcher>();

            services.AddSingleton<IOutboxSender, KafkaOutboxSender>();
            services.AddSingleton(typeof(AS.ISerializer<>), typeof(JsonSerializer<>));

            services.AddSingleton(p =>
            {
                var options = p.GetRequiredService<IOptions<KafkaProducerOptions>>();
                var config = new ProducerConfig { BootstrapServers = string.Join(",", options.Value.BootstrapServers!) };
                return new ProducerBuilder<Null, string>(config).Build();
            });

            services.AddSingleton<IValidateOptions<OutboxHostedServiceOptions>, OutboxHostedServiceOptionsValidator>();
            services.AddSingleton<IValidateOptions<KafkaProducerOptions>, KafkaProducerOptionsValidator>();
            services.AddSingleton<IValidateOptions<KafkaOutboxSenderOptions>, KafkaOutboxSenderOptionsValidation>();

            services.AddScoped<IOutboxMessageProcessor, OutboxMessageProcessor>();
            services.AddScoped<IOutboxUnitOfWork, OutboxUnitOfWork>();

            services.Configure<OutboxHostedServiceOptions>(_configuration.GetSection(nameof(OutboxHostedServiceOptions)));
            services.Configure<KafkaOutboxSenderOptions>(_configuration.GetSection(nameof(KafkaOutboxSenderOptions)));
            services.Configure<KafkaProducerOptions>(_configuration.GetSection(nameof(KafkaProducerOptions)));

            services.AddHostedService<OutboxHostedService>();
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
