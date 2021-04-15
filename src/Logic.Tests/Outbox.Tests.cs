using System;

using Xunit;

using FluentAssertions;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

using Moq;

using Abstractions.DB;


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
			var fetcher =  new Mock<IOutboxFetcher>();
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
	}
}
