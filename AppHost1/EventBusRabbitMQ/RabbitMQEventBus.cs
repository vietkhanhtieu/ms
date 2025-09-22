using EventBus.Abstractions;
using EventBus.Events;
using Microsoft.Extensions.DependencyInjection;
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
        public RabbitMQEventBus(ILogger<RabbitMQEventBus> logger)
        {
            _logger = logger;
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

        public async Task PublishAsync(object @event)
        {
            try
            {
                await EnsureInitializedAsync();

                var runtimeType = @event.GetType();
                var routingKey = runtimeType.Name;

                var json = JsonSerializer.Serialize(@event, runtimeType); // serialize theo runtime type
                var body = Encoding.UTF8.GetBytes(json);

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

        public async Task SubscribeAsync<T, TH>(IServiceProvider services, CancellationToken ct = default)
          where T : IntegrationEvent
          where TH : class, IIntegrationEventHandler<T>
        {
            await EnsureInitializedAsync();

            // 1) Queue + binding
            var eventType = typeof(T).Name;
            var queueName = $"{ExchangeName}.{eventType}";
            await _channel.QueueDeclareAsync(queue: queueName, durable: true, exclusive: false, autoDelete: false);
            await _channel.QueueBindAsync(queue: queueName, exchange: ExchangeName, routingKey: eventType);

            // 2) Prefetch để tránh dồn nhiều message về 1 consumer
            await _channel.BasicQosAsync(0, prefetchCount: 16, global: false);

            // 3) Consumer với manual ACK
            var consumer = new AsyncEventingBasicConsumer(_channel);
            var json = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            consumer.ReceivedAsync += async (_, ea) =>
            {
                try
                {
                    // Deserialize
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var @event = JsonSerializer.Deserialize<T>(message, json);
                    if (@event is null)
                    {
                        // Không parse được → NACK, không requeue (để tránh poison loop)
                        await _channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false);
                        return;
                    }

                    using var scope = services.CreateScope();
                    var handler = scope.ServiceProvider.GetRequiredService<IIntegrationEventHandler<T>>();

                    await handler.Handle(@event);

                    // Thành công → ACK
                    await _channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Error handling event {EventType}", eventType);
                    // Lỗi → NACK (không requeue). Nếu bạn có DLX, message sẽ đi DLQ.
                    await _channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false);
                }
            };

            await _channel.BasicConsumeAsync(queue: queueName, autoAck: false, consumer: consumer);
        }


        //public void Unsubscribe<T, TH>()
        //    where T : class
        //    where TH : IIntegrationEventHandler<T>
        //{
        //    // có thể implement remove subscription
        //}
    }
}
