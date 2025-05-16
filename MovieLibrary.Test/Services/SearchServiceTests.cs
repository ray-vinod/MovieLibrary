using MovieLibrary.Models;
using MovieLibrary.Services;

namespace MovieLibrary.Test.Services;

public class SearchServiceTests
{
    private readonly List<Movie> _sampleMovies;

    public SearchServiceTests()
    {
        _sampleMovies = new List<Movie>
        {
            new Movie { Id = "M001", Title = "Inception" },
            new Movie { Id = "M002", Title = "Interstellar" },
            new Movie { Id = "M003", Title = "The Prestige" },
            new Movie { Id = "M004", Title = "The Dark Knight" },
            new Movie { Id = "M005", Title = "Tenet" }
        };
    }

    [Fact]
    public void SearchMoviesByTitle_ShouldReturnMatchingMovies()
    {
        // Act
        var result = SearchService.SearchMoviesByTitle(_sampleMovies, "In");

        // Assert
        var matched = result.ToList();
        Assert.NotEmpty(matched);
        Assert.Contains(matched, m => m.Title == "Inception");
        Assert.Contains(matched, m => m.Title == "Interstellar");
    }

    [Fact]
    public void SearchMoviesByTitle_ShouldReturnAll_WhenTitleIsNullOrWhiteSpace()
    {
        // Act
        var result1 = SearchService.SearchMoviesByTitle(_sampleMovies, null!);
        var result2 = SearchService.SearchMoviesByTitle(_sampleMovies, "");

        // Assert
        Assert.Equal(5, result1.Count());
        Assert.Equal(5, result2.Count());
    }

    [Fact]
    public void SearchMovieById_ShouldReturnCorrectMovie_WhenIdExists()
    {
        // Act
        var result = SearchService.SearchMovieById(_sampleMovies, "M003");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("M003", result?.Id);
        Assert.Equal("The Prestige", result?.Title);
    }

    [Fact]
    public void SearchMovieById_ShouldReturnNull_WhenIdDoesNotExist()
    {
        // Act
        var result = SearchService.SearchMovieById(_sampleMovies, "M999");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void SearchMovieById_ShouldReturnNull_WhenIdIsEmptyOrNull()
    {
        // Act
        var result1 = SearchService.SearchMovieById(_sampleMovies, "");
        var result2 = SearchService.SearchMovieById(_sampleMovies, null!);

        // Assert
        Assert.Null(result1);
        Assert.Null(result2);
    }
}