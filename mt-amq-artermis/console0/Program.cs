using console0;
using MassTransit;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddMassTransit(x =>
        {
            x.AddConsumer<ExternalConsumer>(typeof(ExternalConsumerDefinition));
            
            x.UsingActiveMq((context,cfg) =>
            {
                // Connect to the openwire port.
                cfg.Host(new Uri("activemq://localhost:9990"), a =>
                {
                    a.Password("artemis");
                    a.Username("artemis");
                });
                
                cfg.ConfigureEndpoints(context);
            });
        });
    })
    .Build();

await host.RunAsync();