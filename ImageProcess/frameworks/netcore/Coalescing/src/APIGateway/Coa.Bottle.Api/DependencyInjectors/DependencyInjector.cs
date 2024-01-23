using System.Reflection;

namespace Coa.Bottle.Api.DependencyInjectors;

public static class DependencyInjector
{
    public static void InstallServices(this IServiceCollection services, IConfiguration configuration,
        params Assembly[] assemblies)
    {
        var installers = assemblies
            .SelectMany(a => a.GetTypes())
            .Where(t => typeof(IServiceInstaller).IsAssignableFrom(t) && t is { IsInterface: false, IsAbstract: false })
            .Select(Activator.CreateInstance)
            .Cast<IServiceInstaller>();

        foreach (var installer in installers) installer.Install(services, configuration);
    }
}