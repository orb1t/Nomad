using System;
using Nomad.Tests.FunctionalTests.ServiceLocation;
using Nomad.ServiceLocation;

public class ResolvingServiceModule : Nomad.Modules.IModuleBootstraper
{
    private IServiceLocator _serviceLocator;

    public ResolvingServiceModule(IServiceLocator serviceLocator)
	{
        _serviceLocator = serviceLocator;
	}

    public void Initialize()
    {
        var serviceProvider = _serviceLocator.Resolve<ITestService>();
        serviceProvider.Execute();
    }

}