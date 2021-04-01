using Abstractions.DB;
using Abstractions.Serialization;
using Abstractions.Service;
using DB;
using Logic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serialization;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

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
            services.AddTransient<IDataProcessor, DataProcessor>();
            services.AddTransient<IProcessingDataUnitOfWork, ProcessingDataUnitOfWork>();
            services.AddSingleton(typeof(IDeserializer<>), typeof(JsonDeserializer<>));
            services.AddSingleton(typeof(ISerializer<>), typeof(JsonSerializer<>));
            services.AddDbContext<OutboxContext>(options => options.UseNpgsql(_configuration.GetConnectionString("DefaultConnection")));
            
            services.AddDbContext<OutboxContext>();

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
