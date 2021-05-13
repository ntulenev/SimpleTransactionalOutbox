﻿using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

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
        public async Task CantProcessNullDataAsync()
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

        public class TestData : IProcessingData
        {
            public long Id { get; set; }

            public int Value { get; set; }
        }

        [Fact(DisplayName = "ProcessingDataUnitOfWork can process new data.")]
        [Trait("Category", "Unit")]
        public async Task CanProcessNewDataAsync()
        {

            // Arrange
            var ctx = _ctx;
            var logger = new Mock<ILogger<ProcessingDataUnitOfWork>>();
            var serializer = new Mock<ISerializer<IProcessingData>>();
            var uow = new ProcessingDataUnitOfWork(ctx, logger.Object, serializer.Object);
            var data = new TestData
            {
                Id = 1,
                Value = 2
            };
            var testJson = "test";
            serializer.Setup(x => x.Serialize(data)).Returns(testJson);

            // Act
            var exception = await Record.ExceptionAsync(async () =>
            {
                await uow.ProcessDataAsync(data, CancellationToken.None);
                await uow.SaveAsync(CancellationToken.None);
            });

            // Assert
            exception.Should().BeNull();

            ctx.ProcessingData.Should().HaveCount(1);
            ctx.ProcessingData.Single().Id.Should().Be(data.Id);
            ctx.ProcessingData.Single().Value.Should().Be(data.Value);

            ctx.OutboxMessages.Should().HaveCount(1);
            ctx.OutboxMessages.Single().Body.Should().Be(testJson);
        }

        [Fact(DisplayName = "ProcessingDataUnitOfWork can rollback new data.")]
        [Trait("Category", "Unit")]
        public async Task CanProcessNewDataRollBackAsync()
        {

            // Arrange
            var ctx = _ctx;
            var logger = new Mock<ILogger<ProcessingDataUnitOfWork>>();
            var serializer = new Mock<ISerializer<IProcessingData>>();

            var data = new TestData
            {
                Id = 1,
                Value = 2
            };
            var testJson = "test";
            serializer.Setup(x => x.Serialize(data)).Returns(testJson);

            // Act
            var exception = await Record.ExceptionAsync(async () =>
            {
                using var uow = new ProcessingDataUnitOfWork(ctx, logger.Object, serializer.Object);
                await uow.ProcessDataAsync(data, CancellationToken.None);
            });

            // Assert
            exception.Should().BeNull();

            ctx.ProcessingData.Should().HaveCount(0);

            ctx.OutboxMessages.Should().HaveCount(0);
        }

        // TODO: Add Can process exists item test
        // TODO: Add Can process exists item test (RollBack)
        // TODO ObjectDisposedException Dispose / DisposeAsync

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