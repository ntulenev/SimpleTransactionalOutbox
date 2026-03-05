using Abstractions.DB;
using Abstractions.Serialization;
using Abstractions.Service;

using System.Diagnostics;

using DB;

using FluentAssertions;

using Logic;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Moq;

using Serialization;

using Xunit;

namespace WebApi.Tests;

public class StartupTests
{
    [Fact(DisplayName = "WebApi Startup registers required services.")]
    [Trait("Category", "Unit")]
    public void ConfigureServicesRegistersRequiredServices()
    {
        // Arrange
        var services = new ServiceCollection();
        var startup = new WebApi.Startup(CreateConfiguration());

        // Act
        var exception = Record.Exception(() => startup.ConfigureServices(services));

        // Assert
        exception.Should().BeNull();

        services.Should().Contain(x => x.ServiceType == typeof(IDataProcessor) && x.ImplementationType == typeof(DataProcessor));
        services.Should().Contain(x => x.ServiceType == typeof(IProcessingDataUnitOfWork) && x.ImplementationType == typeof(ProcessingDataUnitOfWork));
        services.Should().Contain(x => x.ServiceType == typeof(IDeserializer<>) && x.ImplementationType == typeof(JsonDeserializer<>));
        services.Should().Contain(x => x.ServiceType == typeof(ISerializer<>) && x.ImplementationType == typeof(JsonSerializer<>));
    }

    [Fact(DisplayName = "WebApi Startup configures request pipeline.")]
    [Trait("Category", "Unit")]
    public void ConfigureBuildsPipeline()
    {
        // Arrange
        var services = new ServiceCollection();
        var startup = new WebApi.Startup(CreateConfiguration());
        startup.ConfigureServices(services);
        services.AddRouting();
        services.AddMvcCore();
        services.AddSingleton<DiagnosticListener>(_ => new DiagnosticListener("WebApi.Tests"));

        var provider = services.BuildServiceProvider();
        var app = new ApplicationBuilder(provider);
        var env = new Mock<IWebHostEnvironment>(MockBehavior.Strict);

        // Act
        var exception = Record.Exception(() =>
        {
            startup.Configure(app, env.Object);
            _ = app.Build();
        });

        // Assert
        exception.Should().BeNull();
    }

    private static IConfiguration CreateConfiguration()
    {
        var values = new Dictionary<string, string?>
        {
            ["ConnectionStrings:DefaultConnection"] = "Host=localhost;Database=test;Username=test;Password=test"
        };

        return new ConfigurationBuilder()
            .AddInMemoryCollection(values)
            .Build();
    }
}
