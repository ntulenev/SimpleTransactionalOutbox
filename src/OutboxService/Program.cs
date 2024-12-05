using OutboxService;

using Serilog;

var host = Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

var app = host.Build();
app.Run();

