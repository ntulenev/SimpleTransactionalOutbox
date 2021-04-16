using Xunit;

using FluentAssertions;

using Transport.Validation;
using Microsoft.Extensions.Options;

namespace Transport.Tests
{
	public class KafkaOutboxSenderOptionsValidationTests
	{
		[Fact(DisplayName = "KafkaOutboxSenderOptionsValidation can be created.")]
		[Trait("Category", "Unit")]
		public void CanBeCreated()
		{

			// Act
			var exception = Record.Exception(() => new KafkaOutboxSenderOptionsValidation());

			// Assert
			exception.Should().BeNull();
		}

		[Fact(DisplayName = "KafkaOutboxSenderOptionsValidation fails on null options.")]
		[Trait("Category", "Unit")]
		public void FailsIfOptionsIsNull()
		{
			// Assert
			var validator = new KafkaOutboxSenderOptionsValidation();
			var options = (KafkaOutboxSenderOptions)null!;

			// Act
			ValidateOptionsResult result = null!;
			var exception = Record.Exception(() => result = validator.Validate(null!, options));

			// Assert
			exception.Should().BeNull();
			result.Failed.Should().BeTrue();
		}

		[Fact(DisplayName = "KafkaOutboxSenderOptionsValidation fails on null topic.")]
		[Trait("Category", "Unit")]
		public void FailsIfOptionsTopicIsNull()
		{
			// Assert
			var validator = new KafkaOutboxSenderOptionsValidation();
			var options = new KafkaOutboxSenderOptions();

			// Act
			ValidateOptionsResult result = null!;
			var exception = Record.Exception(() => result = validator.Validate(null!, options));

			// Assert
			exception.Should().BeNull();
			result.Failed.Should().BeTrue();
		}

		[Fact(DisplayName = "KafkaOutboxSenderOptionsValidation fails on empty topic.")]
		[Trait("Category", "Unit")]
		public void FailsIfOptionsTopicIsEmpty()
		{
			// Assert
			var validator = new KafkaOutboxSenderOptionsValidation();
			var options = new KafkaOutboxSenderOptions()
			{
				TopicName = string.Empty
			};

			// Act
			ValidateOptionsResult result = null!;
			var exception = Record.Exception(() => result = validator.Validate(null!, options));

			// Assert
			exception.Should().BeNull();
			result.Failed.Should().BeTrue();
		}

		[Fact(DisplayName = "KafkaOutboxSenderOptionsValidation fails on topic with only spaces.")]
		[Trait("Category", "Unit")]
		public void FailsIfOptionsTopicWithInluSpaces()
		{
			// Assert
			var validator = new KafkaOutboxSenderOptionsValidation();
			var options = new KafkaOutboxSenderOptions()
			{
				TopicName = "     "
			};

			// Act
			ValidateOptionsResult result = null!;
			var exception = Record.Exception(() => result = validator.Validate(null!, options));

			// Assert
			exception.Should().BeNull();
			result.Failed.Should().BeTrue();
		}

		[Fact(DisplayName = "KafkaOutboxSenderOptionsValidation fails on topic with spaces.")]
		[Trait("Category", "Unit")]
		public void FailsIfOptionsTopicWithSpaces()
		{
			// Assert
			var validator = new KafkaOutboxSenderOptionsValidation();
			var options = new KafkaOutboxSenderOptions()
			{
				TopicName = "test topic"
			};

			// Act
			ValidateOptionsResult result = null!;
			var exception = Record.Exception(() => result = validator.Validate(null!, options));

			// Assert
			exception.Should().BeNull();
			result.Failed.Should().BeTrue();
		}

		[Fact(DisplayName = "KafkaOutboxSenderOptionsValidation fails on topic with max length.")]
		[Trait("Category", "Unit")]
		public void FailsIfOptionsTopicWithMaxLength()
		{
			// Assert
			var validator = new KafkaOutboxSenderOptionsValidation();
			var options = new KafkaOutboxSenderOptions()
			{
				TopicName = new string('a', 250)
			};

			// Act
			ValidateOptionsResult result = null!;
			var exception = Record.Exception(() => result = validator.Validate(null!, options));

			// Assert
			exception.Should().BeNull();
			result.Failed.Should().BeTrue();
		}

		[Fact(DisplayName = "KafkaOutboxSenderOptionsValidation fails on topic with bad chars.")]
		[Trait("Category", "Unit")]
		public void FailsIfOptionsTopicWithBadChars()
		{
			// Assert
			var validator = new KafkaOutboxSenderOptionsValidation();
			var options = new KafkaOutboxSenderOptions()
			{
				TopicName = "~!@#$%A"
			};

			// Act
			ValidateOptionsResult result = null!;
			var exception = Record.Exception(() => result = validator.Validate(null!, options));

			// Assert
			exception.Should().BeNull();
			result.Failed.Should().BeTrue();
		}

		[Fact(DisplayName = "KafkaOutboxSenderOptionsValidation does not fail on correct topic.")]
		[Trait("Category", "Unit")]
		public void NotFailsIfOptionsIsCorrect()
		{
			// Assert
			var validator = new KafkaOutboxSenderOptionsValidation();
			var options = new KafkaOutboxSenderOptions()
			{
				TopicName = "testname"
			};

			// Act
			ValidateOptionsResult result = null!;
			var exception = Record.Exception(() => result = validator.Validate(null!, options));

			// Assert
			exception.Should().BeNull();
			result.Succeeded.Should().BeTrue();
		}
	}
}
