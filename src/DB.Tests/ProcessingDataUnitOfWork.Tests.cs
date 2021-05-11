using System;

using Xunit;

using FluentAssertions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;

using Moq;

using Abstractions.Models;
using Abstractions.Serialization;

namespace DB.Tests
{
    public class ProcessingDataUnitOfWorkTests : IDisposable
    {

        public ProcessingDataUnitOfWorkTests()
        {
            _conn = new SqliteConnection("Filename=:memory:");
            _conn.Open();
            var builder = new DbContextOptionsBuilder<OutboxContext>();
            builder.UseSqlite(_conn);
            var options = builder.Options;
            _ctx = new OutboxContext(options);
        }

        [Fact(DisplayName = "ProcessingDataUnitOfWork cant be created with null context.")]
        [Trait("Category", "Unit")]
        public void CantCreateWithNullContext()
        {

            // Arrange
            var ctx = (OutboxContext)null!;
            var logger = new Mock<ILogger<ProcessingDataUnitOfWork>>();
            var serializer = new Mock<ISerializer<IProcessingData>>();

            // Act
            var exception = Record.Exception(() => new ProcessingDataUnitOfWork(ctx, logger.Object, serializer.Object));

            // Assert
            exception.Should().NotBeNull().And.BeOfType<ArgumentNullException>();
        }


        [Fact(DisplayName = "ProcessingDataUnitOfWork cant be created with null logger.")]
        [Trait("Category", "Unit")]
        public void CantCreateWithNullLogger()
        {

            // Arrange
            var ctx = _ctx;
            var logger = (ILogger<ProcessingDataUnitOfWork>)null!;
            var serializer = new Mock<ISerializer<IProcessingData>>();

            // Act
            var exception = Record.Exception(() => new ProcessingDataUnitOfWork(ctx, logger, serializer.Object));

            // Assert
            exception.Should().NotBeNull().And.BeOfType<ArgumentNullException>();
        }

        [Fact(DisplayName = "ProcessingDataUnitOfWork cant be created with null serializer.")]
        [Trait("Category", "Unit")]
        public void CantCreateWithNullSerializer()
        {

            // Arrange
            var ctx = _ctx;
            var logger = new Mock<ILogger<ProcessingDataUnitOfWork>>();
            var serializer = (ISerializer<IProcessingData>)null!;

            // Act
            var exception = Record.Exception(() => new ProcessingDataUnitOfWork(ctx, logger.Object, serializer));

            // Assert
            exception.Should().NotBeNull().And.BeOfType<ArgumentNullException>();
        }

        [Fact(DisplayName = "ProcessingDataUnitOfWork can be created.")]
        [Trait("Category", "Unit")]
        public void CanCreateWithValidParams()
        {

            // Arrange
            var ctx = _ctx;
            var logger = new Mock<ILogger<ProcessingDataUnitOfWork>>();
            var serializer = new Mock<ISerializer<IProcessingData>>();

            // Act
            var exception = Record.Exception(() => new ProcessingDataUnitOfWork(ctx, logger.Object, serializer.Object));

            // Assert
            exception.Should().BeNull();
        }



        public void Dispose()
        {
            _ctx.Dispose();
            _conn.Close();
            _conn.Dispose();
        }

        private readonly OutboxContext _ctx;
        private SqliteConnection _conn;
    }
}
