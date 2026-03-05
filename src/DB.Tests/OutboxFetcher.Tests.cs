using Abstractions.Models;

using FluentAssertions;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using Xunit;

namespace DB.Tests;

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
        var options = Options.Create(new OutboxFetcherOptions { Limit = 10 });

        // Act
        var exception = Record.Exception(() => new OutboxFetcher(ctx, options));

        // Assert
        exception.Should().NotBeNull().And.BeOfType<ArgumentNullException>();
    }

    [Fact(DisplayName = "OutboxFetcher cant be created with null options.")]
    [Trait("Category", "Unit")]
    public void CantCreateWithNullOptions()
    {

        // Arrange
        var ctx = _ctx;
        IOptions<OutboxFetcherOptions> options = null!;

        // Act
        var exception = Record.Exception(() => new OutboxFetcher(ctx, options));

        // Assert
        exception.Should().NotBeNull().And.BeOfType<ArgumentNullException>();
    }

    [Fact(DisplayName = "OutboxFetcher can be created with valid context and options.")]
    [Trait("Category", "Unit")]
    public void CanCreateWithValidContext()
    {

        // Arrange
        var ctx = _ctx;
        var options = Options.Create(new OutboxFetcherOptions { Limit = 10 });

        // Act
        var exception = Record.Exception(() => new OutboxFetcher(ctx, options));

        // Assert
        exception.Should().BeNull();
    }

    [Fact(DisplayName = "OutboxFetcher can be created with valid context.")]
    [Trait("Category", "Unit")]
    public async Task CanReadMessagesFromStorageAsync()
    {

        // Arrange
        const int limit = 15;
        var ctx = _ctx;
        var date = DateTime.UtcNow;
        ctx.OutboxMessages.AddRange(Enumerable.Range(0, 100).Select(x => new OutboxMessage
        {
            Body = x.ToString(),
            MessageType = Abstractions.Models.OutboxMessageType.ProcessingDataMessage,
            OccurredOn = date.AddMinutes(x)
        }));
        ctx.SaveChanges();
        var options = Options.Create(new OutboxFetcherOptions { Limit = limit });
        var fetcher = new OutboxFetcher(ctx, options);
        var result = (IEnumerable<IOutboxMessage>)null!;
        using var tokenSource = new CancellationTokenSource();

        // Act
        var exception = await Record.ExceptionAsync(async () => result = await fetcher.ReadOutboxMessagesAsync(tokenSource.Token));

        // Assert
        exception.Should().BeNull();
        int indexer = 0;
        result.Should().HaveCount(limit);
        foreach (var item in result)
        {
            item.Body.Should().Be($"{indexer}");
            item.MessageType.Should().Be(Abstractions.Models.OutboxMessageType.ProcessingDataMessage);
            item.OccurredOn.Should().Be(date.AddMinutes(indexer));
            indexer++;
        }
    }

    private readonly OutboxContext _ctx;
    private readonly SqliteConnection _conn;
}
