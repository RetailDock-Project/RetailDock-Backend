using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Infrastructure.Messaging
{
    public class RabbitMQService : IDisposable
    {
        public IModel Channel { get; }
        private readonly IConnection _connection;

        public RabbitMQService(IOptions<RabbitMQSettings> settings)
        {
            var config = settings.Value;

            var factory = new ConnectionFactory
            {
                HostName = config.HostName,
                UserName = config.UserName,
                Password = config.Password
            };

            _connection = factory.CreateConnection();
            Channel = _connection.CreateModel();

            Channel.ExchangeDeclare(exchange: config.ExchangeName, type: ExchangeType.Direct);
            Channel.QueueDeclare(queue: config.QueueName, durable: true, exclusive: false, autoDelete: false);
            Channel.QueueBind(queue: config.QueueName, exchange: config.ExchangeName, routingKey: config.RoutingKey);
        }

        public void Dispose()
        {
            Channel?.Close();
            _connection?.Close();
        }
    }
}
