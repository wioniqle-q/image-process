using System.Globalization;
using System.Reflection;
using Compunet.YoloV8;
using OpenCvSharp;

namespace ConsoleApp1;

internal static class Program
{
    private static async Task<bool> ProcessImage(YoloV8 model, Mat math)
    {
        var result = await model.DetectAsync(math.ToBytes());
        
        var detections = result.Boxes.ToList();
        var detected = false;

        await Parallel.ForEachAsync(detections, async (detection, token) =>
        {
            if (detection.Confidence < 0.756f)
                return;

            detected = true;

            await Task.Run(() =>
            {
                var x1 = detection.Bounds.X;
                var y1 = detection.Bounds.Y;
                var x2 = detection.Bounds.X + detection.Bounds.Width;
                var y2 = detection.Bounds.Y + detection.Bounds.Height;

                Cv2.Rectangle(math, new Point(x1, y1), new Point(x2, y2), Scalar.Red, 2);
                Cv2.PutText(math, detection.Confidence.ToString(CultureInfo.CurrentCulture), new Point(x1, y1 - 10),
                    HersheyFonts.HersheySimplex, 1, Scalar.Red, 2);
            }, token);
        });

        return await Task.FromResult(detected);
    }

    private static Task Main()
    {
        var exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var modelPath = Path.Combine(exePath!, "best.onnx");

        var model = new YoloV8(modelPath);
        var cap = new VideoCapture(0);

        var width = cap.FrameWidth;
        var height = cap.FrameHeight;

        using var videoWriter = new VideoWriter("output.avi", FourCC.MJPG, 10, new Size(width, height));

        while (true)
        {
            var frame = cap.RetrieveMat();
            if (frame.Empty()) break;

            var x = ProcessImage(model, frame);
            x.Wait();
            Console.WriteLine("Plastic bottle detected: " + x.Result);

            videoWriter.Write(frame);
            Cv2.ImShow("frame", frame);
            
            if (Cv2.WaitKey(1) == 'q')
                break;
        }

        videoWriter.Release();
        cap.Release();
        Cv2.DestroyAllWindows();

        return Task.CompletedTask;
    }
}
