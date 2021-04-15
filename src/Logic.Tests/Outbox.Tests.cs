using System;

using Xunit;

using FluentAssertions;

using Microsoft.Extensions.Logging;

using Moq;

using Abstractions.DB;
using Microsoft.Extensions.DependencyInjection;

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
	}
}
