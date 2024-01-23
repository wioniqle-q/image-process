using Microsoft.AspNetCore.Http;

namespace Bottle.Domain.EntitiesInterfaces;

public interface IProcessImage
{
    public IFormFile ImageFile { get; set; }
}