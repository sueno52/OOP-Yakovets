namespace lab13v6;
public class Result<T>
{
    public bool IsSuccess { get; }
    public T Value { get; }
    public string Error { get; }

    public bool IsFailure => !IsSuccess;

    private Result(T value, bool isSuccess, string error)
    {
        Value = value;
        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result<T> Success(T value)
    {
        return new Result<T>(value, true, string.Empty);
    }

    public static Result<T> Failure(string error)
    {
        return new Result<T>(default!, false, error);
    }

    public static Result<T> Failure(Exception ex)
    {
        return new Result<T>(default!, false, $"{ex.GetType().Name}: {ex.Message}");
    }

    public override string ToString()
    {
        return IsSuccess ? $"Успіх: {Value}" : $"Невдача: {Error}";
    }
}