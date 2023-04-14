using MassTransit;
using MassTransit.Serialization;

namespace mt_activemq_console;

public record ExternalMessage
{
    public string? Value { get; set; }
    public bool? Fail { get; set; }
}

public class ExternalConsumer : IConsumer<ExternalMessage>
{
    public async Task Consume(ConsumeContext<ExternalMessage> context)
    {
        Console.WriteLine("VALUE: " + context.Message.Value);

        if (context.Message.Fail ?? false)
            throw new ApplicationException("Fail");
        
        await Task.CompletedTask;
    }
}

public class ExternalConsumerDefinition : ConsumerDefinition<ExternalConsumer>
{
    public ExternalConsumerDefinition()
    {
        // Point directly at the queue.
        Endpoint(a => a.Name = "foo.bar");
    }
    
    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<ExternalConsumer> consumerConfigurator)
    {
        // Prevents the default message type configuration.
        endpointConfigurator.ConfigureConsumeTopology = false;
        
        // Remove all serialization so we can just use raw json.
        endpointConfigurator.ClearSerialization();
        
        // Use raw json with any message type allowed.
        endpointConfigurator.UseRawJsonSerializer(RawSerializerOptions.AnyMessageType);
        
        endpointConfigurator.UseRetry(a => a.Interval(2, TimeSpan.FromSeconds(1)));
    }
}