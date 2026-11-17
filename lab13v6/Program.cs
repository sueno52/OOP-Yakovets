using System.Text;

namespace lab13v6;

public class Program
{
    public static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

        var processor = new ImageProcessor();

        byte[] goodImage = ImageProcessor.PngSignature.Concat(new byte[32]).ToArray();
        byte[] brokenImage = { 1, 2, 3, 4, 5 };

        Console.WriteLine("1. ResizeImage (власний виняток)");

        Resize(processor, goodImage, "cat.png");
        Resize(processor, brokenImage, "broken.png");
        Resize(processor, null!, "empty.png");

        Console.WriteLine("\n2. ApplyFilter (Result<bool>)");

        PrintFilter(processor, goodImage, "sepia");
        PrintFilter(processor, goodImage, "cartoon");
        PrintFilter(processor, brokenImage, "blur");

        Console.WriteLine("\n3. TryResizeImage (Result<byte[]> замість винятку)");

        var ok = processor.TryResizeImage(goodImage, 64, 64, "dog.png");
        Console.WriteLine(ok.IsSuccess
            ? $"dog.png: зменшено до {ok.Value.Length} байт"
            : $"dog.png: {ok.Error}");

        var fail = processor.TryResizeImage(brokenImage, 64, 64, "broken.png");
        Console.WriteLine(fail.IsSuccess
            ? $"broken.png: зменшено до {fail.Value.Length} байт"
            : $"broken.png: {fail.Error}");

        var badSize = processor.TryResizeImage(goodImage, 0, 64, "cat.png");
        Console.WriteLine($"cat.png 0x64: {badSize}");
    }

    private static void Resize(ImageProcessor processor, byte[] data, string name)
    {
        try
        {
            byte[] result = processor.ResizeImage(data, 200, 100, name);
            Console.WriteLine($"{name}: зменшено до 200x100, {result.Length} байт");
        }
        catch (ImageProcessingException ex)
        {
            Console.WriteLine($"{name}: ПОМИЛКА");
            Console.WriteLine($"   Повідомлення: {ex.Message}");
            Console.WriteLine($"   ImageName = {ex.ImageName}, Operation = {ex.Operation}");
            if (ex.InnerException != null)
                Console.WriteLine($"   Причина (InnerException): {ex.InnerException.GetType().Name}");
        }
    }

    private static void PrintFilter(ImageProcessor processor, byte[] data, string filter)
    {
        Result<bool> result = processor.ApplyFilter(data, filter);

        if (result.IsSuccess)
            Console.WriteLine($"Фільтр '{filter}': застосовано");
        else
            Console.WriteLine($"Фільтр '{filter}': не застосовано — {result.Error}");
    }
}