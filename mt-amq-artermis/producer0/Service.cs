using MassTransit;
using Microsoft.Extensions.Hosting;
using shared;

namespace producer0;

public class Service : IHostedService
{
    private readonly IBus _bus;

    public Service(IBus bus)
    {
        _bus = bus;
    }
    
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var e = await _bus.GetSendEndpoint(new Uri("queue:foo.bar"));
        
        await e.Send(new ExternalMessage()
        {
            Value = "HELLO"
        }, cancellationToken: cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}