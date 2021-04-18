using System;

using Xunit;

using FluentAssertions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;

using Moq;

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

        private readonly OutboxContext _ctx;
        private SqliteConnection _conn;
    }
}
