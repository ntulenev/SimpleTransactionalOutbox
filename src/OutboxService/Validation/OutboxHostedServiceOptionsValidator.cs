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

        if (options.Delay != TimeSpan.Zero)
        {
            return ValidateOptionsResult.Fail($"{nameof(options.Delay)} should be positive.");
        }

        return ValidateOptionsResult.Success;
    }
}
