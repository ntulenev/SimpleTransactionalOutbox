using Microsoft.Extensions.Options;

using OutboxService.Config;

namespace OutboxService.Validation;

/// <summary>
/// Validator for <see cref="OutboxHostedServiceOptions"/>.
/// </summary>
public class OutboxHostedServiceOptionsValidator : IValidateOptions<OutboxHostedServiceOptions>
{
    /// <summary>
    /// Validates configuration.
    /// </summary>
    public ValidateOptionsResult Validate(string? name, OutboxHostedServiceOptions options)
    {
        if (options is null)
        {
            return ValidateOptionsResult.Fail("Configuration object is null.");
        }

        if (options.MinDelay <= TimeSpan.Zero)
        {
            return ValidateOptionsResult.Fail($"{nameof(options.MinDelay)} should be positive.");
        }

        if (options.MaxDelay <= TimeSpan.Zero)
        {
            return ValidateOptionsResult.Fail($"{nameof(options.MaxDelay)} should be positive.");
        }

        if (options.MaxDelay < options.MinDelay)
        {
            return ValidateOptionsResult.Fail($"{nameof(options.MaxDelay)} should be greater than or equal to {nameof(options.MinDelay)}.");
        }

        if (options.StepsCount <= 0)
        {
            return ValidateOptionsResult.Fail($"{nameof(options.StepsCount)} should be positive.");
        }

        return ValidateOptionsResult.Success;
    }
}
