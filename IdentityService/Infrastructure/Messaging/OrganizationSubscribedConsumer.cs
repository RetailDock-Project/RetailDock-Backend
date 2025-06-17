using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Application.Events;
using Application.Interfaces.IServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Infrastructure.Messaging
{
    public class OrganizationSubscribedConsumer: BackgroundService
    {
        private readonly RabbitMQService _rabbitMQService;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly RabbitMQSettings _settings;

        public OrganizationSubscribedConsumer(RabbitMQService rabbitMQService, IServiceScopeFactory scopeFactory, IOptions<RabbitMQSettings> settings)
        {
            _rabbitMQService = rabbitMQService;
            _scopeFactory = scopeFactory;
            _settings = settings.Value;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_rabbitMQService.Channel);

            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var eventData = JsonSerializer.Deserialize<OrganizationSubscribedEvent>(message);

                using var scope = _scopeFactory.CreateScope();
                var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
                await userService.UpdateOrganizationIdandRole(eventData.UserId, eventData.OrganizationId);
            };

            _rabbitMQService.Channel.BasicConsume(queue: _settings.QueueName, autoAck: true, consumer: consumer);
            return Task.CompletedTask;
        }
    }
}
