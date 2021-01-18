using AutoMapper;
using MessagingWorkerService.DtoModels;
using MessagingWorkerService.Models;
using MessagingWorkerService.Options;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Options;
using System;
using System.Data.Common;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace MessagingWorkerService.Services
{
    public class SendMessageService
    {
        private readonly MessagingOptions _messagingOptions;
        private readonly TopicClient topicClient;
        private readonly IMapper _mapper;

        public SendMessageService(
            IMapper mapper,
            IOptions<MessagingOptions> messagingOptions)
        {
            _messagingOptions = messagingOptions.Value;
            _mapper = mapper;

            topicClient = new TopicClient(
                _messagingOptions.ServiceBusConnectionString,
                _messagingOptions.TopicNameToSend);
          
        }
        public async Task SendMessage(
            CancellationToken cancellationToken)
        {
            var order = await GetOrder(cancellationToken);

            var orderDto = _mapper.Map<OrderDto>(order);

            var json = JsonSerializer.Serialize(orderDto);
            var payload = Encoding.UTF8.GetBytes(json);
            var message = new Message(payload);
            
            await topicClient.SendAsync(message);
        }

        private Task<Order> GetOrder(CancellationToken cancellationToken)
        {
            return Task.FromResult(new Order
            {
                Id = 2,
                Description = "Hello from Worker Service",
                OrderDate = DateTimeOffset.Now
            });
        }
    }
}