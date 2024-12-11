using System.ServiceProcess;

namespace ServiceMonitor.Tests;

internal sealed class TestServiceController : IServiceController
{
    internal const string DefaultServiceName = "service";

    public string DisplayName => "Test Service";

    public string ServiceName
    {
        get
        {
            if (_name is null)
            {
                if (!_givenName.Equals(DefaultServiceName, StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException($"Service '{_givenName}' was not found on computer '.'.");
                }
                _name = DefaultServiceName;
            }
            return _name;
        }
    }

    public ServiceControllerStatus Status
    {
        get
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            _status ??= _nextStatus;
            return _status.Value;
        }
    }

    private readonly string _givenName;
    private string? _name;
    private ServiceControllerStatus? _status;
    private ServiceControllerStatus _nextStatus;
    private bool _disposed;

    public TestServiceController() : this(DefaultServiceName)
    {
        _name = DefaultServiceName;
    }

    public TestServiceController(string name)
    {
        _nextStatus = ServiceControllerStatus.Stopped;
        _givenName = name;
    }

    public void Dispose()
    {
        _disposed = true;
    }

    public void Refresh()
    {
        _status = null;
    }

    public void Start()
    {
        _nextStatus = ServiceControllerStatus.Running;
    }

    public void Stop()
    {
        _nextStatus = ServiceControllerStatus.Stopped;
    }
}
