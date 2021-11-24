using Xunit;

using FluentAssertions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;

using Abstractions.Models;

namespace DB.Tests
{
    public class OutboxFetcherTests : IDisposable
    {

        public OutboxFetcherTests()
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

        [Fact(DisplayName = "OutboxFetcher cant be created with null context.")]
        [Trait("Category", "Unit")]
        public void CantCreateWithNullContext()
        {

            // Arrange
            var ctx = (OutboxContext)null!;

            // Act
            var exception = Record.Exception(() => new OutboxFetcher(ctx));

            // Assert
            exception.Should().NotBeNull().And.BeOfType<ArgumentNullException>();
        }

        [Fact(DisplayName = "OutboxFetcher cant be created with valid context.")]
        [Trait("Category", "Unit")]
        public void CanCreateWithValidContext()
        {

            // Arrange
            var ctx = _ctx;

            // Act
            var exception = Record.Exception(() => new OutboxFetcher(ctx));

            // Assert
            exception.Should().BeNull();
        }

        [Fact(DisplayName = "OutboxFetcher cant be created with valid context.")]
        [Trait("Category", "Unit")]
        public async Task CanReadMessagesFromStorageAsync()
        {

            // Arrange
            var ctx = _ctx;
            var date = DateTime.UtcNow;
            ctx.OutboxMessages.AddRange(Enumerable.Range(0, 100).Select(x => new OutboxMessage
            {
                Body = x.ToString(),
                MessageType = Abstractions.Models.OutboxMessageType.ProcessingDataMessage,
                OccurredOn = date.AddMinutes(x)
            }));
            ctx.SaveChanges();
            var fetcher = new OutboxFetcher(ctx);
            var result = (IEnumerable<IOutboxMessage>)null!;

            // Act
            var exception = await Record.ExceptionAsync(async () => result = await fetcher.ReadOutboxMessagesAsync(CancellationToken.None));

            // Assert
            exception.Should().BeNull();
            int indexer = 0;
            result.Should().HaveCount(10);
            foreach (var item in result)
            {
                item.Body.Should().Be($"{indexer}");
                item.MessageType.Should().Be(Abstractions.Models.OutboxMessageType.ProcessingDataMessage);
                item.OccurredOn.Should().Be(date.AddMinutes(indexer));
                indexer++;
            }
        }

        private readonly OutboxContext _ctx;
        private SqliteConnection _conn;
    }
}
