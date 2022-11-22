using Xunit;

using FluentAssertions;

using Microsoft.Extensions.Logging;

using Moq;

using Abstractions.DB;
using Abstractions.Models;


namespace Logic.Tests;

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
        exception.Should().NotBeNull().And.BeOfType<ArgumentNullException>();
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
        exception.Should().NotBeNull().And.BeOfType<ArgumentNullException>();
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


    [Fact(DisplayName = "DataProcessor cant process null data.")]
    [Trait("Category", "Unit")]
    public async Task CantProcessDataWithNullParamAsync()
    {
        // Arrange
        var uow = new Mock<IProcessingDataUnitOfWork>();
        var ilogger = new Mock<ILogger<DataProcessor>>();
        var data = (IProcessingData)null!;
        var p = new DataProcessor(uow.Object, ilogger.Object);

        // Act
        var exception = await Record.ExceptionAsync(async () => await p.ProcessDataAsync(data, CancellationToken.None));

        // Assert
        exception.Should().NotBeNull().And.BeOfType<ArgumentNullException>();

    }

    [Fact(DisplayName = "DataProcessor cant process null data.")]
    [Trait("Category", "Unit")]
    public async Task CanProcessData()
    {
        // Arrange
        var ilogger = new Mock<ILogger<DataProcessor>>();
        var data = new Mock<IProcessingData>(); 
        var token = new CancellationTokenSource();
        var uow = new Mock<IProcessingDataUnitOfWork>();
        int callOrder = 0;
        uow.Setup(x => x.ProcessDataAsync(data.Object, token.Token)).Callback(() => callOrder++.Should().Be(0));
        uow.Setup(x => x.SaveAsync(token.Token)).Callback(() => callOrder++.Should().Be(1));
        var p = new DataProcessor(uow.Object, ilogger.Object);

        // Act
        var exception = await Record.ExceptionAsync(async () => await p.ProcessDataAsync(data.Object, token.Token));

        // Assert
        exception.Should().BeNull();

        uow.Verify(x => x.ProcessDataAsync(data.Object, token.Token), Times.Once);
        uow.Verify(x => x.SaveAsync(token.Token), Times.Once);

    }
}
