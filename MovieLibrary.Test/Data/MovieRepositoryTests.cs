using System;
using Xunit;
using System.Linq;
using System.Collections.Generic;
using MovieLibrary.Data;
using MovieLibrary.Models;

public class MovieRepositoryTests
{
    [Fact]
    public void AddMovie_ShouldAddValidMovie()
    {
        // Arrange
        var repo = new MovieRepository();
        var movie = new Movie
        {
            Id = "M100",
            Title = "Interstellar",
            Director = "Christopher Nolan",
            Genre = "Sci-Fi",
            ReleaseYear = 2014,
            IsAvailable = true
        };

        // Act
        repo.AddMovie(movie);
        var result = repo.GetMovieById("M100");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Interstellar", result.Title);
    }

    [Fact]
    public void AddMovie_DuplicateId_ShouldThrowException()
    {
        // Arrange
        var repo = new MovieRepository();
        var movie1 = new Movie { Id = "M101", Title = "Inception", ReleaseYear = 2010 };
        var movie2 = new Movie { Id = "M101", Title = "Dunkirk", ReleaseYear = 2017 };

        // Act
        repo.AddMovie(movie1);

        // Assert
        var ex = Assert.Throws<ArgumentException>(() => repo.AddMovie(movie2));
        Assert.Contains("already exists", ex.Message);
    }

    [Fact]
    public void AddMovie_SameTitleAndYear_ShouldThrowException()
    {
        // Arrange
        var repo = new MovieRepository();
        var movie1 = new Movie { Id = "M102", Title = "Tenet", ReleaseYear = 2020 };
        var movie2 = new Movie { Id = "M103", Title = "Tenet", ReleaseYear = 2020 };

        // Act
        repo.AddMovie(movie1);

        // Assert
        var ex = Assert.Throws<ArgumentException>(() => repo.AddMovie(movie2));
        Assert.Contains("Title and Release Year are same", ex.Message);
    }

    [Fact]
    public void UpdateMovie_ShouldModifyExistingMovie()
    {
        // Arrange
        var repo = new MovieRepository();
        var movie = new Movie { Id = "M104", Title = "Batman Begins", ReleaseYear = 2005 };
        repo.AddMovie(movie);

        var updatedMovie = new Movie { Id = "M104", Title = "The Dark Knight", ReleaseYear = 2008 };

        // Act
        repo.UpdateMovie(updatedMovie);
        var result = repo.GetMovieById("M104");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("The Dark Knight", result.Title);
        Assert.Equal(2008, result.ReleaseYear);
    }

    [Fact]
    public void DeleteMovie_ShouldRemoveMovie()
    {
        // Arrange
        var repo = new MovieRepository();
        var movie = new Movie { Id = "M105", Title = "The Prestige", ReleaseYear = 2006 };
        repo.AddMovie(movie);

        // Act
        repo.DeleteMovie("M105");
        var result = repo.GetMovieById("M105");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetAllMovies_ShouldReturnAll()
    {
        // Arrange
        var repo = new MovieRepository();
        repo.AddMovie(new Movie { Id = "M106", Title = "Movie 1", ReleaseYear = 2000 });
        repo.AddMovie(new Movie { Id = "M107", Title = "Movie 2", ReleaseYear = 2001 });

        // Act
        var allMovies = repo.GetAllMovies().ToList();

        // Assert
        Assert.Equal(2, allMovies.Count);
        Assert.Contains(allMovies, m => m.Id == "M106");
        Assert.Contains(allMovies, m => m.Id == "M107");
    }

    [Fact]
    public void UpdateMovie_NonExistent_ShouldThrowKeyNotFound()
    {
        // Arrange
        var repo = new MovieRepository();
        var movie = new Movie { Id = "M999", Title = "Unknown Movie", ReleaseYear = 2021 };

        // Act & Assert
        var ex = Assert.Throws<KeyNotFoundException>(() => repo.UpdateMovie(movie));
        Assert.Contains("does not exist", ex.Message);
    }

    [Fact]
    public void DeleteMovie_NonExistent_ShouldThrowKeyNotFound()
    {
        // Arrange
        var repo = new MovieRepository();

        // Act & Assert
        var ex = Assert.Throws<KeyNotFoundException>(() => repo.DeleteMovie("M888"));
        Assert.Contains("does not exist", ex.Message);
    }
}