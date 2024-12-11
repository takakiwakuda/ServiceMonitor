// (c) Takaki Wakuda.

using System.CommandLine.Builder;
using System.CommandLine.Hosting;
using System.CommandLine.Parsing;
using Microsoft.Extensions.DependencyInjection;

namespace ServiceMonitor;

internal static class MonitorCommandLineParser
{
    public static Parser Create() => Create<ServiceControllerFactory, StreamWriterFactory>();

    public static Parser Create<TSCFactory, TTWFactory>()
        where TSCFactory : class, IServiceControllerFactory
        where TTWFactory : class, ITextWriterFactory
    {
        return new CommandLineBuilder(new MonitorCommand()).UseDefaults()
            .UseHost(hostBuilder =>
            {
                hostBuilder.ConfigureServices((context, services) =>
                {
                    services.AddTransient<IServiceControllerFactory, TSCFactory>();
                    services.AddTransient<ITextWriterFactory, TTWFactory>();
                })
                .UseCommandHandler<MonitorCommand, MonitorCommandHandler>();
            })
            .Build();
    }
}
