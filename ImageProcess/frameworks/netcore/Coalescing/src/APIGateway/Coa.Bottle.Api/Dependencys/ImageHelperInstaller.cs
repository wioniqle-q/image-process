using Bottle.Infrastructure.ImageProcess.ImageHandle.ImageAbstractions;
using Bottle.Infrastructure.ImageProcess.ImageHandle.ImageHelpers;
using Bottle.Infrastructure.ImageProcess.ImageHandle.ImageInterfaces;
using Coa.Bottle.Api.DependencyInjectors;

namespace Coa.Bottle.Api.Dependencys;

public sealed class ImageHelperInstaller : IServiceInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<ImageHelper>();
        services.AddSingleton<IImageHelper, ImageHelper>();
        services.AddSingleton<ImageHelperAbstract, ImageHelper>();
    }
}