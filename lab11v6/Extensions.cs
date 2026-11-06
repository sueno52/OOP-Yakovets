namespace lab11v6;

public static class SongExtensions
{
    public static TimeSpan TotalDuration(this IEnumerable<Song> songs)
    {
        int totalSeconds = songs.Sum(s => s.DurationSeconds);
        return TimeSpan.FromSeconds(totalSeconds);
    }

    public static string ToReadable(this TimeSpan time)
    {
        if (time.TotalHours >= 1)
            return $"{(int)time.TotalHours} год {time.Minutes} хв {time.Seconds} с";
        return $"{time.Minutes} хв {time.Seconds} с";
    }
}

public static class EnumerableExtensions
{

    public static IEnumerable<T> Page<T>(this IEnumerable<T> source, int pageNumber, int pageSize)
    {
        if (pageNumber < 1)
            throw new ArgumentOutOfRangeException(nameof(pageNumber), "Номер сторінки має бути від 1");
        if (pageSize < 1)
            throw new ArgumentOutOfRangeException(nameof(pageSize), "Розмір сторінки має бути від 1");

        return source.Skip((pageNumber - 1) * pageSize).Take(pageSize);
    }
}