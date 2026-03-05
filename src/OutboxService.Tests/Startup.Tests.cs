using Abstractions.Bus;
using Abstractions.DB;
using Abstractions.Service;
using System.Diagnostics;

using Confluent.Kafka;

using DB;

using FluentAssertions;

using Logic;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

using Moq;

using OutboxService.Config;
using OutboxService.Services;
using OutboxService.Validation;

using Serialization;

using Transport;
using Transport.Validation;

using Xunit;

using AS = Abstractions.Serialization;

namespace OutboxService.Tests;

public class StartupTests
{
    [Fact(DisplayName = "OutboxService Startup registers required services.")]
    [Trait("Category", "Unit")]
    public void ConfigureServicesRegistersRequiredServices()
    {
        // Arrange
        var services = new ServiceCollection();
        var startup = new OutboxService.Startup(CreateConfiguration());

        // Act
        var exception = Record.Exception(() => startup.ConfigureServices(services));

        // Assert
        exception.Should().BeNull();

        services.Should().Contain(x => x.ServiceType == typeof(IOutbox) && x.ImplementationType == typeof(Outbox));
        services.Should().Contain(x => x.ServiceType == typeof(IOutboxFetcher) && x.ImplementationType == typeof(OutboxFetcher));
        services.Should().Contain(x => x.ServiceType == typeof(IOutboxProcessor) && x.ImplementationType == typeof(OutboxProcessor));
        services.Should().Contain(x => x.ServiceType == typeof(IOutboxSender) && x.ImplementationType == typeof(KafkaOutboxSender));
        services.Should().Contain(x => x.ServiceType == typeof(IOutboxMessageProcessor) && x.ImplementationType == typeof(OutboxMessageProcessor));
        services.Should().Contain(x => x.ServiceType == typeof(IOutboxUnitOfWork) && x.ImplementationType == typeof(OutboxUnitOfWork));

        services.Should().Contain(x => x.ServiceType == typeof(IOutboxBackoffDelayProvider) && x.ImplementationFactory != null);
        services.Should().Contain(x => x.ServiceType == typeof(AS.ISerializer<>) && x.ImplementationType == typeof(JsonSerializer<>));
        services.Should().Contain(x => x.ServiceType == typeof(IHostedService) && x.ImplementationType == typeof(OutboxHostedService));

        services.Should().Contain(x => x.ServiceType == typeof(IValidateOptions<OutboxHostedServiceOptions>) && x.ImplementationType == typeof(OutboxHostedServiceOptionsValidator));
        services.Should().Contain(x => x.ServiceType == typeof(IValidateOptions<OutboxFetcherOptions>) && x.ImplementationType == typeof(OutboxFetcherOptionsValidator));
        services.Should().Contain(x => x.ServiceType == typeof(IValidateOptions<KafkaProducerOptions>) && x.ImplementationType == typeof(KafkaProducerOptionsValidator));
        services.Should().Contain(x => x.ServiceType == typeof(IValidateOptions<KafkaOutboxSenderOptions>) && x.ImplementationType == typeof(KafkaOutboxSenderOptionsValidation));
    }

    [Fact(DisplayName = "OutboxService Startup configures request pipeline.")]
    [Trait("Category", "Unit")]
    public void ConfigureBuildsPipeline()
    {
        // Arrange
        var services = new ServiceCollection();
        var startup = new OutboxService.Startup(CreateConfiguration());
        startup.ConfigureServices(services);
        services.AddRouting();
        services.AddSingleton<DiagnosticListener>(_ => new DiagnosticListener("OutboxService.Tests"));

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

    [Fact(DisplayName = "OutboxService Startup can resolve singleton factory services.")]
    [Trait("Category", "Unit")]
    public void ConfigureServicesResolvesFactoryServices()
    {
        // Arrange
        var services = new ServiceCollection();
        var startup = new OutboxService.Startup(CreateConfiguration());
        startup.ConfigureServices(services);

        using var provider = services.BuildServiceProvider();

        // Act
        var backoffProvider = provider.GetRequiredService<IOutboxBackoffDelayProvider>();
        using var producer = provider.GetRequiredService<IProducer<Null, string>>();

        // Assert
        backoffProvider.Should().NotBeNull();
        producer.Should().NotBeNull();
    }

    private static IConfiguration CreateConfiguration()
    {
        var values = new Dictionary<string, string?>
        {
            ["ConnectionStrings:DefaultConnection"] = "Host=localhost;Database=test;Username=test;Password=test",
            [$"{nameof(OutboxHostedServiceOptions)}:{nameof(OutboxHostedServiceOptions.MinDelay)}"] = "00:00:00.010",
            [$"{nameof(OutboxHostedServiceOptions)}:{nameof(OutboxHostedServiceOptions.MaxDelay)}"] = "00:00:00.500",
            [$"{nameof(OutboxHostedServiceOptions)}:{nameof(OutboxHostedServiceOptions.StepsCount)}"] = "10",
            [$"{nameof(OutboxFetcherOptions)}:{nameof(OutboxFetcherOptions.Limit)}"] = "10",
            [$"{nameof(KafkaOutboxSenderOptions)}:{nameof(KafkaOutboxSenderOptions.TopicName)}"] = "topic-a",
            [$"{nameof(KafkaProducerOptions)}:{nameof(KafkaProducerOptions.BootstrapServers)}:0"] = "localhost:9092"
        };

        return new ConfigurationBuilder()
            .AddInMemoryCollection(values)
            .Build();
    }
}
