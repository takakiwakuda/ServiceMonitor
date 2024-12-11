// (c) Takaki Wakuda.

using System.CommandLine;

namespace ServiceMonitor;

internal sealed class MonitorCommand : RootCommand
{
    public MonitorCommand() : base(CommandDescriptions.MonitorCommand)
    {
        // Setup 'service' argument
        AddArgument(new Argument<string>("service", CommandDescriptions.ServiceArgument));

        // Setup '--output' option
        AddOption(new Option<string>(["--output", "-o"], CommandDescriptions.OutputOption));

        // Setup '--count' option
        Option<uint> countOption = new(["--count", "-c"], () => 1, CommandDescriptions.CountOption);
        countOption.AddValidator(result =>
        {
            if (result.GetValueOrDefault<uint>() < 1)
            {
                result.ErrorMessage = CommandDescriptions.CountValidationError;
            }
        });
        AddOption(countOption);

        // Setup '--force' option
        AddOption(new Option<bool>(["--force", "-f"], CommandDescriptions.ForceOption));
    }
}

internal static class CommandDescriptions
{
    internal static string MonitorCommand = "Monitor and record the status of a Windows service.";
    internal static string ServiceArgument = "Specify the name of a Windows service to monitor";
    internal static string CountOption = "Specify the number of times to monitor the service";
    internal static string CountValidationError = "'--count' must be a positive number (1 or higher).";
    internal static string ForceOption = "Overwrite an existing output";
    internal static string OutputOption = "Specify path to record monitoring results";
}
