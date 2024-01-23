namespace Coa.Bottle.Api.DependencyInjectors;

public interface IServiceInstaller
{
    void Install(IServiceCollection services, IConfiguration configuration);
}