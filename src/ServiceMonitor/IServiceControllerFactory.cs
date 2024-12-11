// (c) Takaki Wakuda.

using System.ServiceProcess;

namespace ServiceMonitor;

internal interface IServiceControllerFactory
{
    IServiceController Create(string name);
}

internal sealed class ServiceControllerFactory : IServiceControllerFactory
{
    public IServiceController Create(string name)
    {
        return new ServiceControllerAdapter(new ServiceController(name));
    }
}
