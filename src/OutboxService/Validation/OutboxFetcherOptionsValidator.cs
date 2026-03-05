using DB;

using Microsoft.Extensions.Options;

namespace OutboxService.Validation;

/// <summary>
/// Validator for <see cref="OutboxFetcherOptions"/>.
/// </summary>
public class OutboxFetcherOptionsValidator : IValidateOptions<OutboxFetcherOptions>
{
    /// <summary>
    /// Validates configuration.
    /// </summary>
    public ValidateOptionsResult Validate(string? name, OutboxFetcherOptions options)
    {
        if (options is null)
        {
            return ValidateOptionsResult.Fail("Configuration object is null.");
        }

        if (options.Limit <= 0)
        {
            return ValidateOptionsResult.Fail($"{nameof(options.Limit)} should be positive.");
        }

        return ValidateOptionsResult.Success;
    }
}
