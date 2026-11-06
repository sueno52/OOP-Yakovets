namespace lab10v6;

public record MovieWithDirector(string Title, string Director);
public record DirectorCount(string Director, int Count);

public static class Queries
{
    public static List<Movie> NolanAfter2000_Query(List<Movie> movies, List<Director> directors) =>
        (from m in movies
         join d in directors on m.DirectorId equals d.Id
         where d.Name == "Christopher Nolan" && m.ReleaseYear > 2000
         orderby m.ReleaseYear
         select m).ToList();

    public static List<Movie> NolanAfter2000_Method(List<Movie> movies, List<Director> directors) =>
        movies
            .Join(directors, m => m.DirectorId, d => d.Id, (m, d) => new { m, d })
            .Where(x => x.d.Name == "Christopher Nolan" && x.m.ReleaseYear > 2000)
            .OrderBy(x => x.m.ReleaseYear)
            .Select(x => x.m)
            .ToList();

    public static List<MovieWithDirector> MoviesWithDirectors_Query(List<Movie> movies, List<Director> directors) =>
        (from m in movies
         join d in directors on m.DirectorId equals d.Id
         select new MovieWithDirector(m.Title, d.Name)).ToList();

    public static List<MovieWithDirector> MoviesWithDirectors_Method(List<Movie> movies, List<Director> directors) =>
        movies
            .Join(directors, m => m.DirectorId, d => d.Id, (m, d) => new MovieWithDirector(m.Title, d.Name))
            .ToList();

    public static List<DirectorCount> CountPerDirector_Query(List<Movie> movies, List<Director> directors) =>
        (from d in directors
         join m in movies on d.Id equals m.DirectorId into directorMovies
         let count = directorMovies.Count()
         orderby count descending, d.Name
         select new DirectorCount(d.Name, count)).ToList();

    public static List<DirectorCount> CountPerDirector_Method(List<Movie> movies, List<Director> directors) =>
        directors
            .GroupJoin(movies, d => d.Id, m => m.DirectorId, (d, ms) => new DirectorCount(d.Name, ms.Count()))
            .OrderByDescending(x => x.Count)
            .ThenBy(x => x.Director)
            .ToList();

    public static Movie Newest_Query(List<Movie> movies) =>
        (from m in movies
         orderby m.ReleaseYear descending
         select m).First();

    public static Movie Newest_Method(List<Movie> movies) =>
        movies.MaxBy(m => m.ReleaseYear)!;
}