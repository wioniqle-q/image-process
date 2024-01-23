using System.Buffers;
using Bottle.Infrastructure.ImageProcess.ImageHandle.ImageAbstractions;
using Compunet.YoloV8;
using Microsoft.AspNetCore.Http;
using OpenCvSharp;
using Image = System.Drawing.Image;

namespace Bottle.Infrastructure.ImageProcess.ImageHandle.ImageHelpers;

public class ImageHelper : ImageHelperAbstract
{
    private static readonly YoloV8 Model = new("");

    public override Task<bool> HandleImageAsync(IFormFile imageFile, CancellationToken cancellationToken = default)
    {
        var result = ProcessImageAsync(imageFile, cancellationToken).Result;
        return Task.FromResult(result);
    }

    private static async ValueTask<bool> ProcessImageAsync(IFormFile imageFile,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var memory = new MemoryStream();
            await imageFile.CopyToAsync(memory, cancellationToken);

            using var img = Image.FromStream(memory);
            memory.Position = 0;

            var buffer = ArrayPool<byte>.Shared.Rent((int)memory.Length);

            try
            {
                var bytesRead = await memory.ReadAsync(buffer, 0, (int)memory.Length, cancellationToken);
                if (bytesRead != memory.Length)
                    throw new Exception("Error reading image data");

                using var mat = Mat.FromImageData(buffer);
                var detected = await DetectImage(Model, mat);

                mat.Dispose();

                return await new ValueTask<bool>(detected);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);

                memory.Position = 0;
                memory.SetLength(0);

                img.Dispose();
                memory.Dispose();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("[ImageHelper] Error: " + ex.Message);
        }

        return await new ValueTask<bool>(false);
    }

    private static async ValueTask<bool> DetectImage(YoloV8 model, Mat image)
    {
        var detectionsPool = ArrayPool<ValueTuple<float, Rect>>.Shared;

        var imageBytes = image.ToBytes();
        var result = await model.DetectAsync(imageBytes);

        var detections = detectionsPool.Rent(result.Boxes.Count);

        for (var i = 0; i < result.Boxes.Count; i++)
        {
            var detection = result.Boxes[i];

            if (!(detection.Confidence >= 0.756)) continue; // 0.75f or 0.756

            var bounds = new Rect(
                detection.Bounds.X,
                detection.Bounds.Y,
                detection.Bounds.Width,
                detection.Bounds.Height
            );

            detections[i] = (detection.Confidence, bounds);
        }

        var detected = false;
        for (var i = 0; i < detections.Length; i++)
        {
            if (detections[i].Equals(default)) continue;
            /*
            var (confidence, bounds) = detections[i];

            var x1 = bounds.X;
            var y1 = bounds.Y;
            var x2 = bounds.X + bounds.Width;
            var y2 = bounds.Y + bounds.Height;

            Cv2.Rectangle(image, new Point(x1, y1), new Point(x2, y2), Scalar.Red, 2);
            Cv2.PutText(image, confidence.ToString(CultureInfo.CurrentCulture), new Point(x1, y1 - 10),
                HersheyFonts.HersheySimplex, 1, Scalar.Red, 2);
            */
            detected = true;
        }

        detectionsPool.Return(detections);

        return await new ValueTask<bool>(detected);
    }
}
