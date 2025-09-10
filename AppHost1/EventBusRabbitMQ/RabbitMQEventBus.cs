using EventBus.Abstractions;
using EventBus.Events;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EventBusRabbitMQ
{
    public class RabbitMQEventBus : IEventBus
    {

        private IConnection? _connection;
        private IChannel? _channel;
        private readonly SemaphoreSlim _initLock = new SemaphoreSlim(1, 1);
        private bool _isInitialized = false;
        private readonly ILogger<RabbitMQEventBus> _logger;

        private const string ExchangeName = "eshop_event_bus";
        public RabbitMQEventBus()
        {
        }

        private async Task EnsureInitializedAsync()
        {
            if (_isInitialized)
                return;

            await _initLock.WaitAsync();
            try
            {
                if (_isInitialized)
                    return;

                var factory = new ConnectionFactory
                {
                    HostName = "localhost",
                    Port = 5672,
                    UserName = "guest",
                    Password = "guest"
                };

                _connection = await factory.CreateConnectionAsync();
                _channel = await _connection.CreateChannelAsync();
                await _channel.ExchangeDeclareAsync(ExchangeName, type: ExchangeType.Direct, durable: true);

                _isInitialized = true;
            }
            finally
            {
                _initLock.Release();
            }
        }

        public async Task PublishAsync<T>(T @event)
        {
            try
            {
                await EnsureInitializedAsync();
                var eventName = @event.GetType().Name;
                var routingKey = typeof(T).Name;
                var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(@event));
                await _channel.BasicPublishAsync(
                    exchange: ExchangeName,
                    routingKey: routingKey,
                    body: body
                );

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error publishing message: {ex.Message}");
            }
        }

        public async Task SubscribeAsync<T, TH>()
          where T : IntegrationEvent
          where TH : class, IIntegrationEventHandler<T>
        {
            await EnsureInitializedAsync();

            var eventType = typeof(T).Name;
            var queueName = $"{ExchangeName}.{eventType}";
            await _channel!.QueueDeclareAsync(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
            await _channel.QueueBindAsync(queue: queueName, exchange: ExchangeName, routingKey: "IntegrationEvent");

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var @event = JsonSerializer.Deserialize<T>(message);
                if (@event != null)
                {
                    var handler = Activator.CreateInstance<TH>();
                    if (handler != null)
                    {
                        await handler.Handle(@event);
                    }
                }
            };
            await _channel.BasicConsumeAsync(queue: queueName, autoAck: true, consumer: consumer);
        }

        //public void Unsubscribe<T, TH>()
        //    where T : class
        //    where TH : IIntegrationEventHandler<T>
        //{
        //    // có thể implement remove subscription
        //}
    }
}
