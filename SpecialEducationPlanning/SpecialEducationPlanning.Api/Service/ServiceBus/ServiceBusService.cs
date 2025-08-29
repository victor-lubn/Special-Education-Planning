using Azure.Messaging.ServiceBus;
using Koa.Serialization.Json;
using Microsoft.Extensions.Logging;
using System;
using System.Text;
using System.Threading.Tasks;

namespace SpecialEducationPlanning
.Api.Service.ServiceBus
{
    public class ServiceBusService : IServiceBusService
    {
        private readonly ILogger<ServiceBusService> _logger;
        private readonly ServiceBusClient _serviceBusClient;
        private readonly IJsonSerializer _serializer;

        public ServiceBusService(
            ServiceBusClient serviceBusClient,
            ILogger<ServiceBusService> logger,
            IJsonSerializer serializer)
        {
            _serviceBusClient = serviceBusClient ?? throw new ArgumentNullException(nameof(serviceBusClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }
        public async Task SendMessageAsync<T>(T messageBody, string queueOrTopicName) where T : class
        {
            await using var sender = _serviceBusClient.CreateSender(queueOrTopicName);

            try
            {
                var messageJson = await _serializer.SerializeAsync(messageBody);

                var message = new ServiceBusMessage(Encoding.UTF8.GetBytes(messageJson))
                {
                    ContentType = "application/json"
                };

                await sender.SendMessageAsync(message);

                _logger.LogInformation("Message successfully sent to {QueueOrTopicName}.", queueOrTopicName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send message to {QueueOrTopicName}.", queueOrTopicName);
                throw;
            }
        }
    }
}
