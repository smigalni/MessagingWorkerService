using MessagingWorkerService.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace MessagingWorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly MessagesService _messagesService;
        private const string ApplicationName = "MessagingWorkerServiceDemo";

        public Worker(ILogger<Worker> logger,
            MessagesService messagesService)
        {
            _logger = logger;
            _messagesService = messagesService;
        }

        public override async Task StartAsync(
            CancellationToken cancellationToken)
        {
            _logger.LogWarning($"Application {ApplicationName} started. ");

            await _messagesService.StartListen();

            await base.StartAsync(cancellationToken);
        }
        protected override async Task ExecuteAsync(
            CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var waitIndefinitely = -1;
                await Task.Delay(waitIndefinitely, cancellationToken);
            }
        }
        public override async Task StopAsync(
           CancellationToken cancellationToken)
        {
            _logger.LogWarning($"Application {ApplicationName} stopped. ");
            await base.StopAsync(cancellationToken);
        }
    }
}