using Microsoft.AspNetCore.Http;

namespace Bottle.Infrastructure.ImageProcess.ImageHandle.ImageInterfaces;

public interface IImageHelper
{
    public Task<bool> HandleImageAsync(IFormFile imageFile, CancellationToken cancellationToken = default);
}