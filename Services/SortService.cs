using MovieLibrary.Models;

namespace MovieLibrary.Services;

public class SortService
{
    // Bubble sort by title
    public static IEnumerable<Movie> BubbleSortByTitle(IEnumerable<Movie> movies)
    {
        List<Movie> sortedMovies = new();
        int n = sortedMovies.Count;

        for (int i = 0; i < n - 1; i++)
        {
            for (int j = 0; j < n - i - 1; j++)
            {
                if (string.Compare(sortedMovies[j].Title, sortedMovies[j + 1].Title, StringComparison.OrdinalIgnoreCase) > 0)
                {
                    // Swap
                    Movie temp = sortedMovies[j];
                    sortedMovies[j] = sortedMovies[j + 1];
                    sortedMovies[j + 1] = temp;
                }
            }
        }

        return sortedMovies;
    }

    // Merge sort by release year
    public static IEnumerable<Movie> MergeSortByReleaseYear(IEnumerable<Movie> movies)
    {
        if (movies.Count() <= 1)
        {
            return movies;
        }

        var mid = movies.Count() / 2;
        var left = MergeSortByReleaseYear(movies.Take(mid));
        var right = MergeSortByReleaseYear(movies.Skip(mid));

        return Merge(left, right);
    }

    private static IEnumerable<Movie> Merge(IEnumerable<Movie> left, IEnumerable<Movie> right)
    {
        List<Movie> result = new();

        while (left.Any() && right.Any())
        {
            if (left.First().ReleaseYear <= right.First().ReleaseYear)
            {
                result.Add(left.First());
                left = left.Skip(1);
            }
            else
            {
                result.Add(right.First());
                right = right.Skip(1);
            }
        }

        result.AddRange(left);
        result.AddRange(right);

        return result;
    }
}