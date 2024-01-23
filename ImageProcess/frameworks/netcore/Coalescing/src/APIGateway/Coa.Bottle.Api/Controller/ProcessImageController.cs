using Bottle.Domain.Entities;
using Bottle.Infrastructure.ImageProcess.ImageHandle.ImageInterfaces;
using Microsoft.AspNetCore.Mvc;

namespace Coa.Bottle.Api.Controller;

[ApiController]
[Route("[controller]")]
public sealed class ProcessImageController(IImageHelper imageHelper) : ControllerBase
{
    [HttpPost("ProcessImage")]
    public async Task<IActionResult> ProcessImage([FromForm] IFormFile imageFile)
    {
        var started = DateTime.Now;

        try
        {
            var imageModel = new BaseProcessImage
            {
                ImageFile = imageFile
            };

            var detected = await imageHelper.HandleImageAsync(imageModel.ImageFile);

            Console.WriteLine("Image processed in " + (DateTime.Now - started).TotalMilliseconds + " ms");
            Console.WriteLine("Image processed " + detected);

            return Ok(detected);
        }
        catch (Exception e)
        {
            Console.WriteLine("Error processing image: " + e.Message);
        }

        return BadRequest();
    }
}