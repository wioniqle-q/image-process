using Bottle.Infrastructure.ImageProcess.ImageHandle.ImageInterfaces;
using Microsoft.AspNetCore.Http;

namespace Bottle.Infrastructure.ImageProcess.ImageHandle.ImageAbstractions;

public abstract class ImageHelperAbstract : IImageHelper
{
    public abstract Task<bool> HandleImageAsync(IFormFile imageFile, CancellationToken cancellationToken = default);
}