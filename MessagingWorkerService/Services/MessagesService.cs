using AutoMapper;
using MessagingWorkerService.DtoModels;
using MessagingWorkerService.Models;
using MessagingWorkerService.Options;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace MessagingWorkerService.Services
{
    public class MessagesService
    {
        private readonly MessagingOptions _messagingOptions;
        private readonly SendMessageService _sendMessageService;
        private readonly IMapper _mapper;
        private readonly ILogger<MessagesService> _logger;
        private readonly SubscriptionClient _subscriptionClient;

        public MessagesService(IOptions<MessagingOptions> messagingOptions,
            SendMessageService sendMessageService,
            IMapper mapper,
            ILogger<MessagesService> logger)
        {
            _messagingOptions = messagingOptions.Value;
            _sendMessageService = sendMessageService;
            _mapper = mapper;
            _logger = logger;

            _subscriptionClient
                = new SubscriptionClient(
                    _messagingOptions.ServiceBusConnectionString,
                    _messagingOptions.TopicNameToListen,
                    _messagingOptions.SubscriptionName);

        }
        public Task StartListen()
        {
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                MaxConcurrentCalls = 1,
                AutoComplete = false
            };

            _subscriptionClient
                .RegisterMessageHandler(ProcessMessagesAsync, messageHandlerOptions);

            return Task.CompletedTask;
        }      

        private async Task ProcessMessagesAsync(Message message,
            CancellationToken cancellationToken)
        {
            var bytesAsString = Encoding.UTF8.GetString(message.Body);

            var orderDto = JsonSerializer.Deserialize<OrderDto>(bytesAsString);

            var order = _mapper.Map<Order>(orderDto);

            await SaveOrder(order, cancellationToken);

            await _subscriptionClient
                .CompleteAsync(message.SystemProperties.LockToken);

            await _sendMessageService.SendMessage(cancellationToken);
        }

        private Task SaveOrder(Order order,
            CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        Task ExceptionReceivedHandler(
            ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            var context = exceptionReceivedEventArgs.ExceptionReceivedContext;

            _logger.LogError($"Message handler encountered an exception" +
                $" {exceptionReceivedEventArgs.Exception}." +
                "Exception context for troubleshooting:" +
                $"- Endpoint: {context.Endpoint}" +
                $"- Entity Path: {context.EntityPath}");

            return Task.CompletedTask;
        }
    }
}