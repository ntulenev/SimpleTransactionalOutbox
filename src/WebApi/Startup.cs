using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

using Abstractions.DB;
using Abstractions.Serialization;
using Abstractions.Service;
using DB;
using Logic;
using Serialization;
using Microsoft.Extensions.Options;

namespace WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IDataProcessor, DataProcessor>();
            services.AddScoped<IProcessingDataUnitOfWork, ProcessingDataUnitOfWork>();

            services.AddSingleton(typeof(IDeserializer<>), typeof(JsonDeserializer<>));
            services.AddSingleton(typeof(ISerializer<>), typeof(JsonSerializer<>));
            
            services.AddDbContext<OutboxContext>(options => options.UseNpgsql(_configuration.GetConnectionString("DefaultConnection")));    

            services.AddControllers();
            services.AddHealthChecks();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/hc");
                endpoints.MapControllers();
            });
        }

        private readonly IConfiguration _configuration;
    }
}
