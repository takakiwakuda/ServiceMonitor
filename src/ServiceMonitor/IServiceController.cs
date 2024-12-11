// (c) Takaki Wakuda.

using System.ServiceProcess;

namespace ServiceMonitor;

internal interface IServiceController : IDisposable
{
    string DisplayName { get; }

    string ServiceName { get; }

    ServiceControllerStatus Status { get; }

    void Refresh();
}

internal sealed class ServiceControllerAdapter(ServiceController controller) : IServiceController
{
    public string DisplayName => _controller.DisplayName;

    public string ServiceName => _controller.ServiceName;

    public ServiceControllerStatus Status => _controller.Status;

    private readonly ServiceController _controller = controller;

    public void Dispose()
    {
        _controller.Dispose();
    }

    public void Refresh()
    {
        _controller.Refresh();
    }
}
