using System.Runtime.Serialization;

namespace lab13v6;
public class ImageProcessingException : Exception
{
    public string ImageName { get; } = string.Empty;
    public string Operation { get; } = string.Empty;

    public ImageProcessingException()
    {
    }

    public ImageProcessingException(string message)
        : base(message)
    {
    }

    public ImageProcessingException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public ImageProcessingException(string imageName, string operation, string reason)
        : base($"Не вдалося виконати '{operation}' для '{imageName}': {reason}")
    {
        ImageName = imageName;
        Operation = operation;
    }

    public ImageProcessingException(string imageName, string operation, string reason, Exception innerException)
        : base($"Не вдалося виконати '{operation}' для '{imageName}': {reason}", innerException)
    {
        ImageName = imageName;
        Operation = operation;
    }

    [Obsolete("Серіалізація винятків застаріла в .NET 8+.", DiagnosticId = "SYSLIB0051")]
    protected ImageProcessingException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}