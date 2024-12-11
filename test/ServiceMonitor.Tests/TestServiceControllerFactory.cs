namespace ServiceMonitor.Tests;

internal sealed class TestServiceControllerFactory : IServiceControllerFactory
{
    public IServiceController Create(string name)
    {
        return new TestServiceController(name);
    }
}
