using System.Collections;
using MovieLibrary.Models;

namespace MovieLibrary.Data;

public class MovieRepository
{
    private readonly Hashtable _movieLookup = new();
    private readonly LinkedList<Movie> _movies = new();

    public void AddMovie(Movie movie)
    {
        if (_movieLookup.ContainsKey(movie.Id!))
        {
            throw new ArgumentException($"Movie with ID {movie.Id} already exists.");
        }

        if (string.IsNullOrWhiteSpace(movie.Id) || string.IsNullOrWhiteSpace(movie.Title))
        {
            throw new ArgumentException("Movie ID and Title cannot be empty.");
        }

        _movieLookup.Add(movie.Id, movie);
        _movies.AddLast(movie);
    }

    public Movie? GetMovieById(string id)
    {
        return _movieLookup.ContainsKey(id) ? _movieLookup[id] as Movie : null;
    }

    public IEnumerable<Movie> GetAllMovies()
    {
        return _movies;
    }

    public void UpdateMovie(Movie movie)
    {
        if (_movieLookup.ContainsKey(movie.Id!))
        {
            _movieLookup[movie.Id!] = movie;
            var existingMovie = _movies.FirstOrDefault(m => m.Id == movie.Id);
            if (existingMovie != null)
            {
                _movies.Remove(existingMovie);
                _movies.AddLast(movie);
            }
        }
    }

    public void DeleteMovie(string id)
    {
        if (_movieLookup.ContainsKey(id))
        {
            if (_movieLookup[id] is Movie movie)
            {
                _movies.Remove(movie);
                _movieLookup.Remove(id);
            }
        }
    }
}