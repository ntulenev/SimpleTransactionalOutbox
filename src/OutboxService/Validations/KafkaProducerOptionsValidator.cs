﻿using System.Linq;

using Microsoft.Extensions.Options;

using OutboxService.Config;

namespace OutboxService.Validations
{
    public class KafkaProducerOptionsValidator : IValidateOptions<KafkaProducerOptions>
    {
        public ValidateOptionsResult Validate(string name, KafkaProducerOptions options)
        {
            if (options is null)
                return ValidateOptionsResult.Fail("Configuration object is null.");

            if (options.BootstrapServers is null)
                return ValidateOptionsResult.Fail($"{nameof(options.BootstrapServers)} is not set.");

            if (!options.BootstrapServers.Any())
                return ValidateOptionsResult.Fail($"{nameof(options.BootstrapServers)} does not contain any item.");

            return ValidateOptionsResult.Success;
        }
    }
}
