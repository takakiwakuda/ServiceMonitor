// (c) Takaki Wakuda.

using System.CommandLine;
using System.CommandLine.Parsing;

namespace ServiceMonitor;

static class Program
{
    static async Task<int> Main(string[] args)
    {
        return await MonitorCommandLineParser.Create().InvokeAsync(args);
    }
}
