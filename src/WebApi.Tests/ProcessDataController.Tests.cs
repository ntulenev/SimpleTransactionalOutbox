using System.Text;

using Abstractions.Serialization;
using Abstractions.Service;

using FluentAssertions;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Models;

using Moq;

using WebApi.Controllers;

using Xunit;

namespace WebApi.Tests;

public class ProcessDataControllerTests
{
    [Fact(DisplayName = "ProcessDataController cant be created with null logger.")]
    [Trait("Category", "Unit")]
    public void CantCreateWithNullLogger()
    {
        // Arrange
        var logger = (ILogger<ProcessDataController>)null!;
        var service = new Mock<IDataProcessor>(MockBehavior.Strict);
        var deserializer = new Mock<IDeserializer<ProcessingData>>(MockBehavior.Strict);
        var lifetime = new Mock<IHostApplicationLifetime>(MockBehavior.Strict);

        // Act
        var exception = Record.Exception(() => _ = new ProcessDataController(logger, service.Object, deserializer.Object, lifetime.Object));

        // Assert
        exception.Should().NotBeNull().And.BeOfType<ArgumentNullException>();
    }

    [Fact(DisplayName = "ProcessDataController cant be created with null service.")]
    [Trait("Category", "Unit")]
    public void CantCreateWithNullService()
    {
        // Arrange
        var logger = new Mock<ILogger<ProcessDataController>>();
        var service = (IDataProcessor)null!;
        var deserializer = new Mock<IDeserializer<ProcessingData>>(MockBehavior.Strict);
        var lifetime = new Mock<IHostApplicationLifetime>(MockBehavior.Strict);

        // Act
        var exception = Record.Exception(() => _ = new ProcessDataController(logger.Object, service, deserializer.Object, lifetime.Object));

        // Assert
        exception.Should().NotBeNull().And.BeOfType<ArgumentNullException>();
    }

    [Fact(DisplayName = "ProcessDataController cant be created with null deserializer.")]
    [Trait("Category", "Unit")]
    public void CantCreateWithNullDeserializer()
    {
        // Arrange
        var logger = new Mock<ILogger<ProcessDataController>>();
        var service = new Mock<IDataProcessor>(MockBehavior.Strict);
        var deserializer = (IDeserializer<ProcessingData>)null!;
        var lifetime = new Mock<IHostApplicationLifetime>(MockBehavior.Strict);

        // Act
        var exception = Record.Exception(() => _ = new ProcessDataController(logger.Object, service.Object, deserializer, lifetime.Object));

        // Assert
        exception.Should().NotBeNull().And.BeOfType<ArgumentNullException>();
    }

    [Fact(DisplayName = "ProcessDataController cant be created with null lifetime.")]
    [Trait("Category", "Unit")]
    public void CantCreateWithNullLifetime()
    {
        // Arrange
        var logger = new Mock<ILogger<ProcessDataController>>();
        var service = new Mock<IDataProcessor>(MockBehavior.Strict);
        var deserializer = new Mock<IDeserializer<ProcessingData>>(MockBehavior.Strict);
        var lifetime = (IHostApplicationLifetime)null!;

        // Act
        var exception = Record.Exception(() => _ = new ProcessDataController(logger.Object, service.Object, deserializer.Object, lifetime));

        // Assert
        exception.Should().NotBeNull().And.BeOfType<ArgumentNullException>();
    }

    [Fact(DisplayName = "ProcessDataController processes decoded request body and returns Ok.")]
    [Trait("Category", "Unit")]
    public async Task ProcessDataAsyncReturnsOkAsync()
    {
        // Arrange
        var logger = new Mock<ILogger<ProcessDataController>>();
        var service = new Mock<IDataProcessor>(MockBehavior.Strict);
        var deserializer = new Mock<IDeserializer<ProcessingData>>(MockBehavior.Strict);
        using var cts = new CancellationTokenSource();
        var lifetime = new Mock<IHostApplicationLifetime>(MockBehavior.Strict);
        lifetime.SetupGet(x => x.ApplicationStopping).Returns(cts.Token);

        var controller = new ProcessDataController(logger.Object, service.Object, deserializer.Object, lifetime.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = CreateHttpContextWithBody("%7B%22id%22%3A5%2C%22value%22%3A7%7D")
            }
        };

        var decodedBody = "{\"id\":5,\"value\":7}";
        var item = new ProcessingData(5, 7);
        var deserializeCalls = 0;
        var processDataCalls = 0;

        deserializer.Setup(x => x.Deserialize(decodedBody))
            .Callback(() => deserializeCalls++)
            .Returns(item);
        service.Setup(x => x.ProcessDataAsync(item, cts.Token))
            .Callback(() => processDataCalls++)
            .Returns(Task.CompletedTask);

        // Act
        var result = await controller.ProcessDataAsync();

        // Assert
        result.Should().NotBeNull().And.BeOfType<OkResult>();
        deserializeCalls.Should().Be(1);
        processDataCalls.Should().Be(1);
    }

    [Fact(DisplayName = "ProcessDataController rethrows exceptions from deserializer.")]
    [Trait("Category", "Unit")]
    public async Task ProcessDataAsyncRethrowsDeserializerErrorAsync()
    {
        // Arrange
        var logger = new Mock<ILogger<ProcessDataController>>();
        var service = new Mock<IDataProcessor>(MockBehavior.Strict);
        var deserializer = new Mock<IDeserializer<ProcessingData>>(MockBehavior.Strict);
        using var cts = new CancellationTokenSource();
        var lifetime = new Mock<IHostApplicationLifetime>(MockBehavior.Strict);
        lifetime.SetupGet(x => x.ApplicationStopping).Returns(cts.Token);

        var controller = new ProcessDataController(logger.Object, service.Object, deserializer.Object, lifetime.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = CreateHttpContextWithBody("body")
            }
        };

        var processDataCalls = 0;
        service.Setup(x => x.ProcessDataAsync(It.IsAny<ProcessingData>(), It.IsAny<CancellationToken>()))
            .Callback(() => processDataCalls++)
            .Returns(Task.CompletedTask);

        deserializer.Setup(x => x.Deserialize("body")).Throws(new InvalidOperationException("boom"));

        // Act
        var exception = await Record.ExceptionAsync(controller.ProcessDataAsync);

        // Assert
        exception.Should().NotBeNull().And.BeOfType<InvalidOperationException>();
        processDataCalls.Should().Be(0);
    }

    private static DefaultHttpContext CreateHttpContextWithBody(string body)
    {
        var context = new DefaultHttpContext();
        context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(body));
        return context;
    }
}
