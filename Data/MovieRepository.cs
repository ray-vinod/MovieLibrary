using System.Collections;
using MovieLibrary.Models;

namespace MovieLibrary.Data;

public class MovieRepository
{
    private Hashtable movieLookup = new();
    private LinkedList<Movie> movies = new();

    public MovieRepository()
    {
        // Initialize with some sample movies
        AddMovie(new Movie("M001", "Inception", "Sci-Fi", 2010, "Christopher Nolan"));
        AddMovie(new Movie("M002", "The Godfather", "Crime", 1972, "Francis Ford Coppola"));
        AddMovie(new Movie("M003", "The Dark Knight", "Action", 2008, "Christopher Nolan"));
    }

    public void AddMovie(Movie movie)
    {
        if (movieLookup.ContainsKey(movie.Id))
        {
            throw new ArgumentException($"Movie with ID {movie.Id} already exists.");
        }

        if (string.IsNullOrWhiteSpace(movie.Id) || string.IsNullOrWhiteSpace(movie.Title))
        {
            throw new ArgumentException("Movie ID and Title cannot be empty.");
        }

        movieLookup.Add(movie.Id, movie);
        movies.AddLast(movie);
    }

    public Movie? GetMovieById(string id)
    {
        return movieLookup.ContainsKey(id) ? movieLookup[id] as Movie : null;
    }

    public IEnumerable<Movie> GetAllMovies()
    {
        return movies;
    }

    public void UpdateMovie(Movie movie)
    {
        if (movieLookup.ContainsKey(movie.Id))
        {
            movieLookup[movie.Id] = movie;
            var existingMovie = movies.FirstOrDefault(m => m.Id == movie.Id);
            if (existingMovie != null)
            {
                movies.Remove(existingMovie);
                movies.AddLast(movie);
            }
        }
    }

    public void DeleteMovie(string id)
    {
        if (movieLookup.ContainsKey(id))
        {
            Movie? movie = movieLookup[id] as Movie;
            if (movie != null)
            {
                movies.Remove(movie);
                movieLookup.Remove(id);
            }
        }
    }

}