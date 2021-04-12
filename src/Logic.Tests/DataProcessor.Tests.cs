using System;

using Xunit;

using FluentAssertions;

using Microsoft.Extensions.Logging;

using Moq;

using Abstractions.DB;

namespace Logic.Tests
{
    public class DataProcessorTests
    {
        [Fact(DisplayName = "DataProcessor cant be created with null UOW.")]
        [Trait("Category", "Unit")]
        public void CantCreateWithNullUOW()
        {

            // Arrange
            var uow = (IProcessingDataUnitOfWork)null!;
            var ilogger = new Mock<ILogger<DataProcessor>>();

            // Act
            var exception = Record.Exception(() => new DataProcessor(uow, ilogger.Object));

            // Assert
            object p = exception.Should().NotBeNull().And.BeOfType<ArgumentNullException>();
        }

        [Fact(DisplayName = "DataProcessor cant be created with null logger.")]
        [Trait("Category", "Unit")]
        public void CantCreateWithNullLogger()
        {

            // Arrange
            var uow = new Mock<IProcessingDataUnitOfWork>();
            var ilogger = (ILogger<DataProcessor>)null!;

            // Act
            var exception = Record.Exception(() => new DataProcessor(uow.Object, ilogger));

            // Assert
            object p = exception.Should().NotBeNull().And.BeOfType<ArgumentNullException>();
        }

        [Fact(DisplayName = "DataProcessor can be created with valid params.")]
        [Trait("Category", "Unit")]
        public void CanCreate()
        {

            // Arrange
            var uow = new Mock<IProcessingDataUnitOfWork>();
            var ilogger = new Mock<ILogger<DataProcessor>>();

            // Act
            var exception = Record.Exception(() => new DataProcessor(uow.Object, ilogger.Object));

            // Assert
            object p = exception.Should().BeNull();
        }
    }
}
