using System;
using System.Threading.Tasks;
using System.Threading;

using Xunit;

using FluentAssertions;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

using Moq;

using Abstractions.DB;
using Abstractions.Service;

namespace Logic.Tests
{
    public class OutboxTests
    {

        [Fact(DisplayName = "Outbox cant be created with null fetcher.")]
        [Trait("Category", "Unit")]
        public void CantCreateWithNullFetcher()
        {
            // Arrange
            var fetcher = (IOutboxFetcher)null!;
            var scopedFactory = new Mock<IServiceScopeFactory>();
            var ilogger = new Mock<ILogger<Outbox>>();

            // Act
            var exception = Record.Exception(() => new Outbox(fetcher, scopedFactory.Object, ilogger.Object));

            // Assert
            exception.Should().NotBeNull().And.BeOfType<ArgumentNullException>();
        }


        [Fact(DisplayName = "Outbox cant be created with null scopedFactory.")]
        [Trait("Category", "Unit")]
        public void CantCreateWithNullFactory()
        {
            // Arrange
            var fetcher = new Mock<IOutboxFetcher>();
            var scopedFactory = (IServiceScopeFactory)null!;
            var ilogger = new Mock<ILogger<Outbox>>();

            // Act
            var exception = Record.Exception(() => new Outbox(fetcher.Object, scopedFactory, ilogger.Object));

            // Assert
            exception.Should().NotBeNull().And.BeOfType<ArgumentNullException>();
        }

        [Fact(DisplayName = "Outbox cant be created with null Logger.")]
        [Trait("Category", "Unit")]
        public void CantCreateWithNullLogger()
        {
            // Arrange
            var fetcher = new Mock<IOutboxFetcher>();
            var scopedFactory = new Mock<IServiceScopeFactory>();
            var ilogger = (ILogger<Outbox>)null!;

            // Act
            var exception = Record.Exception(() => new Outbox(fetcher.Object, scopedFactory.Object, ilogger));

            // Assert
            exception.Should().NotBeNull().And.BeOfType<ArgumentNullException>();
        }

        [Fact(DisplayName = "Outbox can be created with valid params.")]
        [Trait("Category", "Unit")]
        public void CanCreate()
        {
            // Arrange
            var fetcher = new Mock<IOutboxFetcher>();
            var scopedFactory = new Mock<IServiceScopeFactory>();
            var ilogger = new Mock<ILogger<Outbox>>();

            // Act
            var exception = Record.Exception(() => new Outbox(fetcher.Object, scopedFactory.Object, ilogger.Object));

            // Assert
            exception.Should().BeNull();
        }

        [Fact(DisplayName = "Outbox process messages correctly.")]
        [Trait("Category", "Unit")]
        public async Task RunProcessingAsyncProcessMessagesCorrectlyAsync()
        {
            // Arrange
            var fetcher = new Mock<IOutboxFetcher>();
            var scopedFactory = new Mock<IServiceScopeFactory>();
            var ilogger = new Mock<ILogger<Outbox>>();
            var outBox = new Outbox(fetcher.Object, scopedFactory.Object, ilogger.Object);
            var tokenSource = new CancellationTokenSource();

            var msg = new TestMessage();
            fetcher.Setup(x => x.ReadOutboxMessagesAsync(tokenSource.Token)).ReturnsAsync(new[] { msg });
            var scope = new Mock<IServiceScope>();
            scopedFactory.Setup(x => x.CreateScope()).Returns(scope.Object);
            var provider = new Mock<IServiceProvider>();
            scope.Setup(x => x.ServiceProvider).Returns(provider.Object);
            var processor = new Mock<IOutboxMessageProcessor>();
            provider.Setup(x => x.GetService(typeof(IOutboxMessageProcessor))).Returns(processor.Object);

            // Act
            var exception = await Record.ExceptionAsync(async () => await outBox.RunProcessingAsync(tokenSource.Token));

            // Assert
            exception.Should().BeNull();
            fetcher.Verify(x => x.ReadOutboxMessagesAsync(tokenSource.Token), Times.Once);
            scopedFactory.Verify(x => x.CreateScope(), Times.Once);
            scope.Verify(x => x.ServiceProvider, Times.Once);
            provider.Verify(x => x.GetService(typeof(IOutboxMessageProcessor)), Times.Once);
            processor.Verify(x => x.TryProcessAsync(msg, tokenSource.Token), Times.Once);
            scope.Verify(x => x.Dispose(), Times.Once);
        }
    }
}
