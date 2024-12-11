using System.ServiceProcess;

namespace ServiceMonitor.Tests;

public class ServiceInfoTests : IDisposable
{
    private readonly TestServiceController _controller;
    private readonly ServiceInfo _serviceInfo;

    public ServiceInfoTests()
    {
        _controller = new TestServiceController();
        _serviceInfo = new ServiceInfo(_controller);
    }

    public void Dispose()
    {
        _controller.Dispose();
        _serviceInfo.Dispose();
    }

    [Fact]
    public void Constructor_WithServiceController_GetExpectedValue()
    {
        Assert.Equal(_controller.DisplayName, _serviceInfo.DisplayName);
        Assert.Equal(_controller.ServiceName, _serviceInfo.ServiceName);
        Assert.Equal(_controller.Status, _serviceInfo.Status);
        Assert.NotEqual(default, _serviceInfo.LastUpdateTime);
    }

    [Fact]
    public void LastUpdateTime_NoRefresh_ReturnsSameValue()
    {
        var time = _serviceInfo.LastUpdateTime;

        Assert.Equal(time, _serviceInfo.LastUpdateTime);
    }

    [Fact]
    public void LastUpdateTime_WhenRefreshed_ReturnsNewValue()
    {
        var time = _serviceInfo.LastUpdateTime;

        _serviceInfo.Refresh();

        Assert.True(time < _serviceInfo.LastUpdateTime);
    }

    [Fact]
    public void Dispose_AfterObjectAccess_ThrowsObjectDisposedException()
    {
        _serviceInfo.Dispose();

        Assert.Throws<ObjectDisposedException>(() => _controller.Status);
    }

    [Fact]
    public void Refresh_WhenStartAndStop_ReturnsExpectedValue()
    {
        _controller.Start();
        Assert.Equal(ServiceControllerStatus.Running, _serviceInfo.Status);

        _controller.Stop();
        Assert.Equal(ServiceControllerStatus.Running, _serviceInfo.Status);

        _serviceInfo.Refresh();
        Assert.Equal(ServiceControllerStatus.Stopped, _serviceInfo.Status);
    }
}
