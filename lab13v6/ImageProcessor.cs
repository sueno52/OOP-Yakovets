namespace lab13v6;
public class ImageProcessor
{
    public static readonly byte[] PngSignature = { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };

    private static readonly HashSet<string> KnownFilters =
        new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "grayscale", "sepia", "invert", "blur" };

    public byte[] ResizeImage(byte[] imageData, int width, int height, string imageName = "без назви")
    {
        if (width <= 0 || height <= 0)
            throw new ArgumentOutOfRangeException(nameof(width), "Ширина і висота мають бути більшими за 0.");

        CheckImage(imageData, imageName, "Resize");

        var resized = new byte[PngSignature.Length + width * height];
        Array.Copy(PngSignature, resized, PngSignature.Length);
        return resized;
    }

    public Result<bool> ApplyFilter(byte[] imageData, string filterName)
    {
        if (string.IsNullOrWhiteSpace(filterName) || !KnownFilters.Contains(filterName))
            return Result<bool>.Failure(
                $"Невідомий фільтр '{filterName}'. Доступні: {string.Join(", ", KnownFilters)}");

        try
        {
            CheckImage(imageData, "зображення", $"Filter:{filterName}");
        }
        catch (ImageProcessingException ex)
        {
            return Result<bool>.Failure(ex); 
        }

        return Result<bool>.Success(true);
    }

    public Result<byte[]> TryResizeImage(byte[] imageData, int width, int height, string imageName = "без назви")
    {
        try
        {
            return Result<byte[]>.Success(ResizeImage(imageData, width, height, imageName));
        }
        catch (Exception ex) when (ex is ImageProcessingException || ex is ArgumentOutOfRangeException)
        {
            return Result<byte[]>.Failure(ex);
        }
    }

    private static void CheckImage(byte[]? imageData, string imageName, string operation)
    {
        if (imageData == null)
            throw new ImageProcessingException(imageName, operation, "дані відсутні",
                new ArgumentNullException(nameof(imageData)));

        if (imageData.Length < PngSignature.Length ||
            !imageData.Take(PngSignature.Length).SequenceEqual(PngSignature))
            throw new ImageProcessingException(imageName, operation, "дані пошкоджені (це не PNG)");
    }
}