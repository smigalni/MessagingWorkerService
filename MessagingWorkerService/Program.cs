using AutoMapper;
using MessagingWorkerService;
using MessagingWorkerService.Options;
using MessagingWorkerService.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

CreateHostBuilder(args).Build().Run();

IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureServices((hostContext, services) =>
        {
            services.Configure<MessagingOptions>(
                          hostContext
                              .Configuration
                                  .GetSection(MessagingOptions.Messaging));

            services.AddTransient<MessagesService>();
            services.AddTransient<SendMessageService>();

            services.AddAutoMapper(typeof(Worker));

            services.AddHostedService<Worker>();
        });