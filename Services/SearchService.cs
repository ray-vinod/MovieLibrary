using MovieLibrary.Models;

namespace MovieLibrary.Services;

public class SearchService
{
    // Linear search for movies based on title
    public static IEnumerable<Movie> SearchMovies(IEnumerable<Movie> movies, string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return movies;
        }

        return movies.Where(movie =>
            movie.Title!.Contains(title, StringComparison.OrdinalIgnoreCase));
    }

    // Binary search for movies based on Id
    public static Movie? SearchMovieById(IEnumerable<Movie> movies, string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return null;
        }

        // Ensure the movies are sorted by Id for binary search
        var sortedMovies = movies.OrderBy(m => m.Id).ToList();
        int left = 0;
        int right = sortedMovies.Count - 1;

        while (left <= right)
        {
            int mid = left + (right - left) / 2;

            if (sortedMovies[mid].Id!.Equals(id, StringComparison.OrdinalIgnoreCase))
            {
                return sortedMovies[mid];
            }

            if (string.Compare(sortedMovies[mid].Id, id, StringComparison.OrdinalIgnoreCase) < 0)
            {
                left = mid + 1;
            }
            else
            {
                right = mid - 1;
            }
        }

        return null;
    }
}