﻿using MassTransit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using mt_activemq_console;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddMassTransit(x =>
        {
            x.AddConsumer<ExternalConsumer>(typeof(ExternalConsumerDefinition));
            
            x.UsingActiveMq((context,cfg) =>
            {
                // Connect to the openwire port.
                cfg.Host("localhost", 61616, a =>
                {
                    a.Password("admin");
                    a.Username("admin");
                });
                
                cfg.ConfigureEndpoints(context);
            });
        });
    })
    .Build();

// var root = host.Services.GetRequiredService<IConfigurationRoot>();
// var debugView = root.GetDebugView();
//
// Console.WriteLine(debugView);

await host.RunAsync();


