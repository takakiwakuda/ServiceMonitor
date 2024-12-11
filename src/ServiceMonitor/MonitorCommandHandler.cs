// (c) Takaki Wakuda.

using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.IO;
using System.Diagnostics;

namespace ServiceMonitor;

internal sealed class MonitorCommandHandler : ICommandHandler
{
    public string? Service { get; set; }

    public string? Output { get; set; }

    public uint Count { get; set; }

    public bool Force { get; set; }

    private readonly IServiceControllerFactory _controller;
    private readonly ITextWriterFactory _textWriter;

    public MonitorCommandHandler(IServiceControllerFactory controller, ITextWriterFactory textWriter)
    {
        _controller = controller;
        _textWriter = textWriter;
    }

    public int Invoke(InvocationContext context)
    {
        throw new NotSupportedException();
    }

    public async Task<int> InvokeAsync(InvocationContext context)
    {
        try
        {
            await ProcessHandler(context);
        }
        catch (OperationCanceledException ex)
        {
            context.Console.Error.WriteLine(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            context.Console.Error.WriteLine(ex.Message);
            return ExitStatus.Failure;
        }
        catch (IOException ex)
        {
            context.Console.Error.WriteLine(ex.Message);
            return ExitStatus.Failure;
        }

        // Show end message
        ShowEnd(context.Console);

        // Return exit status code
        return ExitStatus.Success;
    }

    private async Task ProcessHandler(InvocationContext context)
    {
        Debug.Assert(Service is not null, $"{nameof(Service)} should not be null.");

        // If '--output' is not specified, write a file into the current directory
        string path = Path.GetFullPath(Output ?? $"{Service}-{DateTime.Today:yyyyMMdd}.csv");
        using TextWriter textWriter = _textWriter.Create(path, Force);
        using ServiceInfo serviceInfo = new(_controller.Create(Service));

        // Show startup message
        ShowStartup(context.Console, serviceInfo, path);

        // Write CSV headers
        await textWriter.WriteLineAsync(ServiceInfo.CsvHeader);

        CancellationToken token = context.GetCancellationToken();
        TimeSpan interval = TimeSpan.FromMinutes(1);
        uint remainingCount = Count;

        while (true)
        {
            // Write a CSV record
            await textWriter.WriteLineAsync(serviceInfo.ToCsvString());

            // Exit when Count=1
            remainingCount--;
            if (remainingCount == 0)
            {
                return;
            }

            // Set next run time
            DateTime nextRunTime = serviceInfo.LastUpdateTime.UtcDateTime + interval;
            TimeSpan delay = nextRunTime - DateTime.UtcNow;
            if (delay > TimeSpan.Zero)
            {
                await Task.Delay(delay, token);
            }

            // Refresh service status
            serviceInfo.Refresh();
        }
    }

    private static void ShowStartup(IConsole console, ServiceInfo serviceInfo, string path)
    {
        console.Out.WriteLine($"""
        --------------------------------------------
           Windows Service Monitor
        --------------------------------------------

          Started : {DateTime.Now:F}
           Target : {serviceInfo.DisplayName} ({serviceInfo.ServiceName})
           Output : {path}

        --------------------------------------------

        """);
    }

    private static void ShowEnd(IConsole console)
    {
        console.Out.WriteLine($"""

        --------------------------------------------

            Ended : {DateTime.Now:F}
        """);
    }
}
