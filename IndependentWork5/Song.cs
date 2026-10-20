namespace IndependentWork5;
public class Song
{
    public string Title { get; }
    public string Artist { get; }
    public int DurationSeconds { get; }

    public Song(string title, string artist, int durationSeconds)
    {
        Title = title;
        Artist = artist;
        DurationSeconds = durationSeconds;
    }
    public override string ToString()
    {
        return $"Пісня: {Artist} — \"{Title}\" ({DurationSeconds / 60}:{DurationSeconds % 60:D2})";
    }
}