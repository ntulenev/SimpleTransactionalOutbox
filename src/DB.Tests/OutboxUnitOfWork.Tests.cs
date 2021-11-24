using Xunit;

using FluentAssertions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;

using Moq;

using Abstractions.Models;


namespace DB.Tests
{
    public class OutboxUnitOfWorkTests : IDisposable
    {

        public OutboxUnitOfWorkTests()
        {
            _conn = new SqliteConnection("Filename=:memory:");
            _conn.Open();
            var builder = new DbContextOptionsBuilder<OutboxContext>();
            builder.UseSqlite(_conn);
            var options = builder.Options;
            _ctx = new OutboxContext(options);
        }

        public void Dispose()
        {
            _ctx.Dispose();
            _conn.Close();
            _conn.Dispose();
        }

        [Fact(DisplayName = "OutboxUnitOfWork cant be created with null context.")]
        [Trait("Category", "Unit")]
        public void CantCreateWithNullContext()
        {

            // Arrange
            var ctx = (OutboxContext)null!;
            var logger = new Mock<ILogger<OutboxUnitOfWork>>();

            // Act
            var exception = Record.Exception(() => new OutboxUnitOfWork(ctx, logger.Object));

            // Assert
            exception.Should().NotBeNull().And.BeOfType<ArgumentNullException>();
        }

        [Fact(DisplayName = "OutboxUnitOfWork cant be created with null logger.")]
        [Trait("Category", "Unit")]
        public void CantCreateWithNullLogger()
        {

            // Arrange
            var ctx = _ctx;
            var logger = (ILogger<OutboxUnitOfWork>)null!;

            // Act
            var exception = Record.Exception(() => new OutboxUnitOfWork(ctx, logger));

            // Assert
            exception.Should().NotBeNull().And.BeOfType<ArgumentNullException>();
        }

        [Fact(DisplayName = "OutboxUnitOfWork can be created.")]
        [Trait("Category", "Unit")]
        public void CanCreateWithValidParams()
        {

            // Arrange
            var ctx = _ctx;
            var logger = new Mock<ILogger<OutboxUnitOfWork>>();

            // Act
            var exception = Record.Exception(() => new OutboxUnitOfWork(ctx, logger.Object));

            // Assert
            exception.Should().BeNull();
        }

        [Fact(DisplayName = "OutboxUnitOfWork can't remove empty message.")]
        [Trait("Category", "Unit")]
        public async Task CantRemoveEmptyMessageAsync()
        {

            // Arrange
            var ctx = _ctx;
            var logger = new Mock<ILogger<OutboxUnitOfWork>>();
            var outbox = new OutboxUnitOfWork(ctx, logger.Object);

            // Act
            var exception = await Record.ExceptionAsync(async () => await outbox.RemoveOutboxMessageAsync(null!, CancellationToken.None));

            // Assert
            exception.Should().NotBeNull().And.BeOfType<ArgumentNullException>();
        }

        [Fact(DisplayName = "OutboxUnitOfWork can't work if disposed.")]
        [Trait("Category", "Unit")]
        public async Task CantWorkWithDisposedObjectAsync()
        {

            // Arrange
            var ctx = _ctx;
            var logger = new Mock<ILogger<OutboxUnitOfWork>>();
            var outbox = new OutboxUnitOfWork(ctx, logger.Object);
            outbox.Dispose();

            // Act
            var exception = await Record.ExceptionAsync(async () => await outbox.RemoveOutboxMessageAsync(new TestMessage(), CancellationToken.None));

            // Assert
            exception.Should().NotBeNull().And.BeOfType<ObjectDisposedException>();
        }

        [Fact(DisplayName = "OutboxUnitOfWork can't save if disposed.")]
        [Trait("Category", "Unit")]
        public async Task CantSaveWithDisposedObjectAsync()
        {

            // Arrange
            var ctx = _ctx;
            var logger = new Mock<ILogger<OutboxUnitOfWork>>();
            var outbox = new OutboxUnitOfWork(ctx, logger.Object);
            outbox.Dispose();

            // Act
            var exception = await Record.ExceptionAsync(async () => await outbox.SaveAsync(CancellationToken.None));

            // Assert
            exception.Should().NotBeNull().And.BeOfType<ObjectDisposedException>();
        }

        [Fact(DisplayName = "OutboxUnitOfWork can't work if disposed async.")]
        [Trait("Category", "Unit")]
        public async Task CantWorkWithDisposedObjectAsync2()
        {

            // Arrange
            var ctx = _ctx;
            var logger = new Mock<ILogger<OutboxUnitOfWork>>();
            var outbox = new OutboxUnitOfWork(ctx, logger.Object);
            await outbox.DisposeAsync();

            // Act
            var exception = await Record.ExceptionAsync(async () => await outbox.RemoveOutboxMessageAsync(new TestMessage(), CancellationToken.None));

            // Assert
            exception.Should().NotBeNull().And.BeOfType<ObjectDisposedException>();
        }

        [Fact(DisplayName = "OutboxUnitOfWork can't save if disposed async.")]
        [Trait("Category", "Unit")]
        public async Task CantSaveWithDisposedObjectAsync2()
        {

            // Arrange
            var ctx = _ctx;
            var logger = new Mock<ILogger<OutboxUnitOfWork>>();
            var outbox = new OutboxUnitOfWork(ctx, logger.Object);
            await outbox.DisposeAsync();

            // Act
            var exception = await Record.ExceptionAsync(async () => await outbox.SaveAsync(CancellationToken.None));

            // Assert
            exception.Should().NotBeNull().And.BeOfType<ObjectDisposedException>();
        }

        [Fact(DisplayName = "OutboxUnitOfWork can remove message.")]
        [Trait("Category", "Unit")]
        public async Task CanRemoveMessageAsync()
        {

            // Arrange
            var ctx = _ctx;
            var msg1 = new OutboxMessage
            {
                Body = "test1",
                MessageType = Abstractions.Models.OutboxMessageType.ProcessingDataMessage,
                OccurredOn = DateTime.UtcNow
            };
            var msg2 = new OutboxMessage
            {
                Body = "test2",
                MessageType = Abstractions.Models.OutboxMessageType.ProcessingDataMessage,
                OccurredOn = DateTime.UtcNow.AddDays(1)
            };
            ctx.OutboxMessages.Add(msg1);
            ctx.OutboxMessages.Add(msg2);
            ctx.SaveChanges();
            var logger = new Mock<ILogger<OutboxUnitOfWork>>();
            var outbox = new OutboxUnitOfWork(ctx, logger.Object);

            // Act
            var exception = await Record.ExceptionAsync(async () =>
            {
                await outbox.RemoveOutboxMessageAsync(new TestMessage
                {
                    MessageId = msg1.MessageId
                }, CancellationToken.None);
                await outbox.SaveAsync(CancellationToken.None);
            });

            // Assert
            exception.Should().BeNull();
            ctx.OutboxMessages.Should().HaveCount(1);
            ctx.OutboxMessages.Single().Should().Be(msg2);
        }

        [Fact(DisplayName = "OutboxUnitOfWork cant remove message without saving.")]
        [Trait("Category", "Unit")]
        public async Task CantRemoveMessageWithoutSavingAsync()
        {

            // Arrange
            var ctx = _ctx;
            var msg = new OutboxMessage
            {
                Body = "test",
                MessageType = Abstractions.Models.OutboxMessageType.ProcessingDataMessage,
                OccurredOn = DateTime.UtcNow
            };
            ctx.OutboxMessages.Add(msg);
            ctx.SaveChanges();
            var logger = new Mock<ILogger<OutboxUnitOfWork>>();
            var outbox = new OutboxUnitOfWork(ctx, logger.Object);

            // Act
            var exception = await Record.ExceptionAsync(async () =>
            {
                await outbox.RemoveOutboxMessageAsync(new TestMessage
                {
                    MessageId = msg.MessageId
                }, CancellationToken.None);
            });

            // Assert
            exception.Should().BeNull();
            ctx.OutboxMessages.Should().NotBeEmpty();
        }

        private readonly OutboxContext _ctx;
        private SqliteConnection _conn;

        private class TestMessage : IOutboxMessage
        {
            public Guid MessageId { get; set; }

            public DateTime OccurredOn { get; set; }

            public OutboxMessageType MessageType { get; set; }

            public string Body { get; set; } = null!;
        }
    }
}
