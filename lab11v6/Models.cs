namespace lab11v6;

public class Artist
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class Song
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int DurationSeconds { get; set; }
    public Artist Artist { get; set; } = null!;
    public string DurationText => $"{DurationSeconds / 60}:{DurationSeconds % 60:D2}";
}

public class Playlist
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<Song> Songs { get; set; } = new List<Song>();   
}
public static class Data
{
    public static List<Artist> Artists { get; } = new List<Artist>
    {
        new Artist { Id = 1, Name = "Океан Ельзи" },
        new Artist { Id = 2, Name = "Jamala" },
        new Artist { Id = 3, Name = "Go_A" },
        new Artist { Id = 4, Name = "Kalush Orchestra" },
        new Artist { Id = 5, Name = "Queen" },
        new Artist { Id = 6, Name = "Coldplay" },
        new Artist { Id = 7, Name = "Imagine Dragons" },
        new Artist { Id = 8, Name = "Adele" },
        new Artist { Id = 9, Name = "Daft Punk" }
    };

    public static List<Song> Songs { get; } = new List<Song>
    {
        new Song { Id = 1,  Title = "Обійми",              DurationSeconds = 245, Artist = Artists[0] },
        new Song { Id = 2,  Title = "Без бою",             DurationSeconds = 290, Artist = Artists[0] },
        new Song { Id = 3,  Title = "1944",                DurationSeconds = 180, Artist = Artists[1] },
        new Song { Id = 4,  Title = "Шум",                 DurationSeconds = 182, Artist = Artists[2] },
        new Song { Id = 5,  Title = "Stefania",            DurationSeconds = 177, Artist = Artists[3] },
        new Song { Id = 6,  Title = "Bohemian Rhapsody",   DurationSeconds = 355, Artist = Artists[4] },
        new Song { Id = 7,  Title = "Don't Stop Me Now",   DurationSeconds = 209, Artist = Artists[4] },
        new Song { Id = 8,  Title = "Yellow",              DurationSeconds = 266, Artist = Artists[5] },
        new Song { Id = 9,  Title = "Viva la Vida",        DurationSeconds = 242, Artist = Artists[5] },
        new Song { Id = 10, Title = "Believer",            DurationSeconds = 204, Artist = Artists[6] },
        new Song { Id = 11, Title = "Thunder",             DurationSeconds = 187, Artist = Artists[6] },
        new Song { Id = 12, Title = "Hello",               DurationSeconds = 295, Artist = Artists[7] },
        new Song { Id = 13, Title = "Rolling in the Deep", DurationSeconds = 228, Artist = Artists[7] },
        new Song { Id = 14, Title = "Відпусти",            DurationSeconds = 260, Artist = Artists[0] },
        new Song { Id = 15, Title = "Get Lucky",           DurationSeconds = 248, Artist = Artists[8] },
        new Song { Id = 16, Title = "Fix You",             DurationSeconds = 295, Artist = Artists[5] },
        new Song { Id = 17, Title = "We Will Rock You",    DurationSeconds = 122, Artist = Artists[4] },
        new Song { Id = 18, Title = "Someone Like You",    DurationSeconds = 285, Artist = Artists[7] }
    };

    public static List<Playlist> Playlists { get; } = new List<Playlist>
    {
        new Playlist { Id = 1, Name = "Українське",    Songs = ById(1, 2, 3, 4, 5, 14) },
        new Playlist { Id = 2, Name = "Рок-класика",   Songs = ById(6, 7, 17, 8) },
        new Playlist { Id = 3, Name = "Для тренувань", Songs = ById(10, 11, 5, 7, 15) },
        new Playlist { Id = 4, Name = "Спокійне",      Songs = ById(12, 13, 18, 16, 9, 1) }
    };
    private static List<Song> ById(params int[] ids) =>
        ids.Select(id => Songs.First(s => s.Id == id)).ToList();
}