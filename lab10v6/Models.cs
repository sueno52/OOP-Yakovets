namespace lab10v6;

public class Director
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
}

public class Movie
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int DirectorId { get; set; }
    public int ReleaseYear { get; set; }
}

public static class Data
{
    public static List<Director> Directors { get; } = new List<Director>
    {
        new Director { Id = 1,  Name = "Christopher Nolan",  Country = "UK" },
        new Director { Id = 2,  Name = "Quentin Tarantino",  Country = "USA" },
        new Director { Id = 3,  Name = "Steven Spielberg",   Country = "USA" },
        new Director { Id = 4,  Name = "Denis Villeneuve",   Country = "Canada" },
        new Director { Id = 5,  Name = "Greta Gerwig",       Country = "USA" },
        new Director { Id = 6,  Name = "Bong Joon-ho",       Country = "South Korea" },
        new Director { Id = 7,  Name = "Hayao Miyazaki",     Country = "Japan" },
        new Director { Id = 8,  Name = "James Cameron",      Country = "Canada" },
        new Director { Id = 9,  Name = "Martin Scorsese",    Country = "USA" },
        new Director { Id = 10, Name = "Alexander Dovzhenko", Country = "Ukraine" }
    };

    public static List<Movie> Movies { get; } = new List<Movie>
    {
        new Movie { Id = 1,  Title = "Following",                  DirectorId = 1,  ReleaseYear = 1998 },
        new Movie { Id = 2,  Title = "Memento",                    DirectorId = 1,  ReleaseYear = 2000 },
        new Movie { Id = 3,  Title = "Inception",                  DirectorId = 1,  ReleaseYear = 2010 },
        new Movie { Id = 4,  Title = "Interstellar",               DirectorId = 1,  ReleaseYear = 2014 },
        new Movie { Id = 5,  Title = "Dunkirk",                    DirectorId = 1,  ReleaseYear = 2017 },
        new Movie { Id = 6,  Title = "Oppenheimer",                DirectorId = 1,  ReleaseYear = 2023 },
        new Movie { Id = 7,  Title = "Pulp Fiction",               DirectorId = 2,  ReleaseYear = 1994 },
        new Movie { Id = 8,  Title = "Once Upon a Time in Hollywood", DirectorId = 2, ReleaseYear = 2019 },
        new Movie { Id = 9,  Title = "Jurassic Park",              DirectorId = 3,  ReleaseYear = 1993 },
        new Movie { Id = 10, Title = "The Fabelmans",              DirectorId = 3,  ReleaseYear = 2022 },
        new Movie { Id = 11, Title = "Arrival",                    DirectorId = 4,  ReleaseYear = 2016 },
        new Movie { Id = 12, Title = "Dune: Part Two",             DirectorId = 4,  ReleaseYear = 2024 },
        new Movie { Id = 13, Title = "Little Women",               DirectorId = 5,  ReleaseYear = 2019 },
        new Movie { Id = 14, Title = "Barbie",                     DirectorId = 5,  ReleaseYear = 2023 },
        new Movie { Id = 15, Title = "Parasite",                   DirectorId = 6,  ReleaseYear = 2019 },
        new Movie { Id = 16, Title = "Spirited Away",              DirectorId = 7,  ReleaseYear = 2001 },
        new Movie { Id = 17, Title = "The Boy and the Heron",      DirectorId = 7,  ReleaseYear = 2023 },
        new Movie { Id = 18, Title = "Titanic",                    DirectorId = 8,  ReleaseYear = 1997 },
        new Movie { Id = 19, Title = "Avatar",                     DirectorId = 8,  ReleaseYear = 2009 },
        new Movie { Id = 20, Title = "Killers of the Flower Moon", DirectorId = 9,  ReleaseYear = 2023 },
        new Movie { Id = 21, Title = "Earth",                      DirectorId = 10, ReleaseYear = 1930 }
    };
}