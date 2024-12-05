using System.Text.RegularExpressions;

using Microsoft.Extensions.Options;

namespace Transport.Validation;

/// <summary>
/// Validator for <see cref="KafkaOutboxSenderOptions"/>.
/// </summary>
public class KafkaOutboxSenderOptionsValidation : IValidateOptions<KafkaOutboxSenderOptions>
{
    /// <summary>
    /// Validates configuration.
    /// </summary>
    public ValidateOptionsResult Validate(string? name, KafkaOutboxSenderOptions options)
    {
        if (options is null)
        {
            return ValidateOptionsResult.Fail("Configuration object is null.");
        }

        if (options.TopicName is null)
        {
            return ValidateOptionsResult.Fail($"{nameof(options.TopicName)} is not set.");
        }

        if (string.IsNullOrWhiteSpace(options.TopicName))
        {
            return ValidateOptionsResult.Fail($"{nameof(options.TopicName)} cannot be empty or consist of whitespaces.");
        }

        if (options.TopicName.Any(character => char.IsWhiteSpace(character)))
        {
            return ValidateOptionsResult.Fail($"{nameof(options.TopicName)} cannot contain whitespaces.");
        }

        if (options.TopicName.Length > MAX_TOPIC_NAME_LENGTH)
        {
            return ValidateOptionsResult.Fail($"{nameof(options.TopicName)} name is too long.");
        }

        if (!_topicNameCharacters.IsMatch(options.TopicName))
        {
            return ValidateOptionsResult.Fail($"{nameof(options.TopicName)} may consist of characters 'a' to 'z', 'A' to 'Z', digits, and minus signs.");
        }

        return ValidateOptionsResult.Success;
    }

    private static readonly Regex _topicNameCharacters = new Regex(
        "^[a-zA-Z0-9\\-]*$",
        RegexOptions.Compiled);

    private const int MAX_TOPIC_NAME_LENGTH = 249;
}
