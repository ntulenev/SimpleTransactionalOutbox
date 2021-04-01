using System;
using System.IO;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Abstractions.Serialization;
using Abstractions.Service;

using Models;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("")]
    public class ProcessDataController : ControllerBase
    {
        public ProcessDataController(
           ILogger<ProcessDataController> logger,
           IDataProcessor service,
           IDeserializer<ProcessingData> deserializer,
           IHostApplicationLifetime lifetime)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _deserializer = deserializer ?? throw new ArgumentNullException(nameof(deserializer));
            _applicationLifetime = lifetime ?? throw new ArgumentNullException(nameof(lifetime));
        }

        [HttpPost]
        public async Task<IActionResult> ProcessDataAsync()
        {
            using var loggingScope = _logger.BeginScope(
               "Processing a push message \"{TraceGUID}\".",
               Guid.NewGuid());

            try
            {
                var body = await ReadRequestBodyAsync();
                var item = _deserializer.Deserialize(body);

                await _service.ProcessDataAsync(item, _applicationLifetime.ApplicationStopping);

                _logger?.LogInformation("Processed a message.");

                return Ok();
            }
            catch (Exception ex)
            {
                _logger?.LogError(
                   ex,
                   "Failed to process the push message.");

                throw;
            }
        }

        private async Task<string> ReadRequestBodyAsync()
        {
            var body = string.Empty;
            using (var reader = new StreamReader(Request.Body))
            {
                body = await reader.ReadToEndAsync();
            }

            return System.Web.HttpUtility.UrlDecode(body);
        }

        private readonly ILogger<ProcessDataController> _logger;
        private readonly IDataProcessor _service;
        private readonly IDeserializer<ProcessingData> _deserializer;
        private readonly IHostApplicationLifetime _applicationLifetime;
    }
}
