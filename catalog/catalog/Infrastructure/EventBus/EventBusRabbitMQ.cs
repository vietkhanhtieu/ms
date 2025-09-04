using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace catalog.Infrastructure.EventBus
{
    public class EventBusRabbitMQ : IEventBus
    {
        private readonly string _exchangeName = string.Empty;
        private IConnection? _connection;
        private IChannel? _channel;
        private readonly SemaphoreSlim _initLock = new SemaphoreSlim(1, 1);
        private bool _isInitialized = false;

        public EventBusRabbitMQ(string exchangeName)
        {
            _exchangeName = exchangeName;
        }

        private async Task EnsureInitializedAsync()
        {
            if (_isInitialized) return;

            await _initLock.WaitAsync();
            try
            {
                if (_isInitialized) return;

                var factory = new ConnectionFactory() { HostName = "localhost" };
                _connection = await factory.CreateConnectionAsync();
                _channel = await _connection.CreateChannelAsync();
                await _channel.ExchangeDeclareAsync(exchange: _exchangeName, type: ExchangeType.Direct, durable: true);

                _isInitialized = true;
            }
            finally
            {
                _initLock.Release();
            }
        }

        public async Task PublishAsync<T>(T @event)
        {
            await EnsureInitializedAsync();

            var eventType = @event.GetType().Name;
            var routingKey = typeof(T).Name;
            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(@event));
            await _channel!.BasicPublishAsync(exchange: _exchangeName, routingKey: routingKey, body: body);
        }

        public async Task SubscribeAsync<T, TH>()
            where T : class
            where TH : IIntegrationEventHandler<T>
        {
            await EnsureInitializedAsync();

            var eventType = typeof(T).Name;
            var queueName = $"{_exchangeName}.{eventType}";
            await _channel!.QueueDeclareAsync(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
            await _channel.QueueBindAsync(queue: queueName, exchange: _exchangeName, routingKey: eventType);

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

        public void Unsubscribe<T, TH>()
            where T : class
            where TH : IIntegrationEventHandler<T>
        {
            // có thể implement remove subscription
        }
    }
}
