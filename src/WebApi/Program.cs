using Serilog;

using WebApi;

var host = Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>()
                      .UseSerilog((hostingContext, loggerConfiguration)
                            => loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration));
                });

var app = host.Build();
app.Run();
