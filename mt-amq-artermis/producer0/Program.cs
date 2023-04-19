using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using producer0;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddMassTransit(x =>
        {
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
        services.AddHostedService<Service>();
    })
    .Build();

await host.RunAsync();