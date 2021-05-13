using System;
using System.Threading;
using System.Threading.Tasks;

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

        [Fact(DisplayName = "ProcessingDataUnitOfWork can't process null data.")]
        [Trait("Category", "Unit")]
        public async Task CanProcessNullDataAsync()
        {

            // Arrange
            var ctx = _ctx;
            var logger = new Mock<ILogger<ProcessingDataUnitOfWork>>();
            var serializer = new Mock<ISerializer<IProcessingData>>();
            var uow = new ProcessingDataUnitOfWork(ctx, logger.Object, serializer.Object);
            var data = (IProcessingData)null!;
            // Act
            var exception = await Record.ExceptionAsync(() => uow.ProcessDataAsync(data, CancellationToken.None));

            // Assert
            exception.Should().NotBeNull().And.BeOfType<ArgumentNullException>();
        }

        // TODO: Add Can process new item test
        // TODO: Add Can process exists item test


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
