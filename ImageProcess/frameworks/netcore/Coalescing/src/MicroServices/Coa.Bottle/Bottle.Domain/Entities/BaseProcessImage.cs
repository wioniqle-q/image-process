using Bottle.Domain.EntitiesAbstracts;
using Microsoft.AspNetCore.Http;

namespace Bottle.Domain.Entities;

public sealed class BaseProcessImage : ProcessImageAbstract
{
    public override IFormFile ImageFile { get; set; } = null!;
}