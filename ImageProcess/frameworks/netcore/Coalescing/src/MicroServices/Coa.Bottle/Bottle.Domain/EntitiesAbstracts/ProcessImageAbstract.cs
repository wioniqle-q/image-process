using Bottle.Domain.EntitiesInterfaces;
using Microsoft.AspNetCore.Http;

namespace Bottle.Domain.EntitiesAbstracts;

public abstract class ProcessImageAbstract : IProcessImage
{
    public virtual IFormFile ImageFile { get; set; } = null!;
}