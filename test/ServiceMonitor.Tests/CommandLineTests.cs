using System.CommandLine;
using System.CommandLine.IO;
using System.CommandLine.Parsing;

namespace ServiceMonitor.Tests;

public class CommandLineTests
{
    private const string ServiceName = TestServiceController.DefaultServiceName;
    private const string IncorrectName = "No service";
    private readonly string OutputPath = Path.GetFullPath($"{ServiceName}-{DateTime.Today:yyyyMMdd}.csv");

    private readonly TestConsole _console = new();

    [Fact]
    public async Task InvokeAsync_WithValidArguments_ReturnsSuccess()
    {
        string path = Path.GetFullPath(nameof(InvokeAsync_WithValidArguments_ReturnsSuccess));
        string[] args = [ServiceName, "--output", path];

        var result = await CreateParser<StringWriterFactory>().InvokeAsync(args, _console);

        AssertSuccess(result, path);
    }

    [Fact]
    public async Task InvokeAsync_NoArguments_ReturnsFailure()
    {
        string expected = "Required argument missing for command: ";

        var result = await CreateParser<StringWriterFactory>().InvokeAsync([], _console);

        AssertFailure(result, expected);
    }

    [Fact]
    public async Task InvokeAsync_InvalidCountOption_ReturnsFailure()
    {
        string expected = "'--count' must be a positive number (1 or higher).";

        var result = await CreateParser<StringWriterFactory>().InvokeAsync([ServiceName, "--count", "0"], _console);

        AssertFailure(result, expected);
    }

    [Fact]
    public async Task InvokeAsync_ServiceNotFound_ReturnsFailure()
    {
        string expected = $"Service '{IncorrectName}' was not found on computer '.'.";

        var result = await CreateParser<StringWriterFactory>().InvokeAsync([IncorrectName], _console);

        AssertFailure(result, expected);
    }

    [Fact]
    public async Task InvokeAsync_FileAlreadyExists_ReturnsFailure()
    {
        string expected = $"The file '{OutputPath}' already exists.";

        var result = await CreateParser<FileAlreadyExistThrower>().InvokeAsync([ServiceName], _console);

        AssertFailure(result, expected);
    }

    [Fact]
    public async Task InvokeAsync_ExistingFileOverwritten_ReturnsSuccess()
    {
        string path = Path.GetFullPath(nameof(InvokeAsync_ExistingFileOverwritten_ReturnsSuccess));
        string[] args = [ServiceName, "--output", path, "--force"];

        var result = await CreateParser<FileAlreadyExistThrower>().InvokeAsync(args, _console);

        AssertSuccess(result, path);
    }

    private void AssertFailure(int exitStatus, string errorMessage)
    {
        Assert.Equal(ExitStatus.Failure, exitStatus);
        Assert.StartsWith(errorMessage, _console.Error.ToString(), StringComparison.Ordinal);
    }

    private static void AssertSuccess(int exitStatus, string path)
    {
        const string expected = $"""
        {ServiceInfo.CsvHeader}
        {ServiceName},Stopped,
        """;

        Assert.Equal(ExitStatus.Success, exitStatus);
        Assert.StartsWith(expected, StringWriterFactory.GetWriter(path).ToString());
    }

    private static Parser CreateParser<T>() where T : class, ITextWriterFactory
    {
        return MonitorCommandLineParser.Create<TestServiceControllerFactory, T>();
    }
}
