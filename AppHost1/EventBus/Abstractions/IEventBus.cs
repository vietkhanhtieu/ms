using EventBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Abstractions
{
    public interface IEventBus
    {
        Task PublishAsync(object @event);

        Task SubscribeAsync<T, TH>(IServiceProvider services, CancellationToken ct = default)
            where T : IntegrationEvent
            where TH : class, IIntegrationEventHandler<T>;
    }
}
