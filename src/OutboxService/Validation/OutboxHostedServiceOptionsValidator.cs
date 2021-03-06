using Microsoft.Extensions.Options;

using OutboxService.Config;


namespace OutboxService.Validation
{
    /// <summary>
    /// Validator for <see cref="OutboxHostedServiceOptions"/>.
    /// </summary>
    public class OutboxHostedServiceOptionsValidator : IValidateOptions<OutboxHostedServiceOptions>
    {
        /// <summary>
        /// Validates configuration.
        /// </summary>
        public ValidateOptionsResult Validate(string name, OutboxHostedServiceOptions options)
        {
            if (options is null)
            {
                return ValidateOptionsResult.Fail("Configuration object is null.");
            }

            if (options.DelayInSeconds <= 0)
            {
                return ValidateOptionsResult.Fail($"{nameof(options.DelayInSeconds)} should be positive.");
            }

            return ValidateOptionsResult.Success;
        }
    }
}
