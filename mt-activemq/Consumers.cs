using MassTransit;
using MassTransit.Serialization;

namespace mt_activemq_console;

public record ExternalMessage
{
    public string? Value { get; set; }
}

public class ExternalConsumer : IConsumer<ExternalMessage>
{
    public async Task Consume(ConsumeContext<ExternalMessage> context)
    {
        Console.WriteLine("VALUE: " + context.Message.Value);
        
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
    }
}