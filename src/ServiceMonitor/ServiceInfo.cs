// (c) Takaki Wakuda.

using System.ServiceProcess;

namespace ServiceMonitor;

internal sealed class ServiceInfo(IServiceController controller) : IDisposable
{
    internal const string CsvHeader = "Name,Status,DateTime";

    public string DisplayName => _controller.DisplayName;

    public string ServiceName => _controller.ServiceName;

    public ServiceControllerStatus Status => _controller.Status;

    public DateTimeOffset LastUpdateTime
    {
        get
        {
            _lastUpdateTime ??= DateTimeOffset.Now;
            return _lastUpdateTime.Value;
        }
    }

    private readonly IServiceController _controller = controller;
    private DateTimeOffset? _lastUpdateTime;

    public void Dispose()
    {
        _controller.Dispose();
    }

    public void Refresh()
    {
        _controller.Refresh();
        _lastUpdateTime = null;
    }

    public string ToCsvString()
    {
        return $"{ServiceName},{Status},{LastUpdateTime:O}";
    }
}
