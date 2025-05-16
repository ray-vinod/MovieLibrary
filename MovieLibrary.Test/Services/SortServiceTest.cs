using MovieLibrary.Models;
using MovieLibrary.Services;

namespace MovieLibrary.Test.Services;

public class SortServiceTests
{
    private readonly List<Movie> _unsortedMovies;

    public SortServiceTests()
    {
        _unsortedMovies = new List<Movie>
        {
            new Movie { Id = "M003", Title = "The Prestige", ReleaseYear = 2006 },
            new Movie { Id = "M001", Title = "Inception", ReleaseYear = 2010 },
            new Movie { Id = "M005", Title = "Tenet", ReleaseYear = 2020 },
            new Movie { Id = "M004", Title = "The Dark Knight", ReleaseYear = 2008 },
            new Movie { Id = "M002", Title = "Interstellar", ReleaseYear = 2014 }
        };
    }

    [Fact]
    public void BubbleSortByTitle_ShouldSortMoviesAlphabetically()
    {
        // Act
        var result = SortService.BubbleSortByTitle(_unsortedMovies).ToList();

        // Assert
        var expectedOrder = new List<string> { "Inception", "Interstellar", "Tenet", "The Dark Knight", "The Prestige" };
        Assert.Equal(expectedOrder, result.Select(m => m.Title));
    }

    [Fact]
    public void MergeSortByReleaseYear_ShouldSortMoviesByYearAscending()
    {
        // Act
        var result = SortService.MergeSortByReleaseYear(_unsortedMovies).ToList();

        // Assert
        var expectedOrder = new List<int> { 2006, 2008, 2010, 2014, 2020 };
        Assert.Equal(expectedOrder, result.Select(m => m.ReleaseYear));
    }

    [Fact]
    public void BubbleSortByTitle_ShouldReturnEmptyList_WhenGivenEmptyList()
    {
        // Arrange
        var emptyList = new List<Movie>();

        // Act
        var result = SortService.BubbleSortByTitle(emptyList);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void MergeSortByReleaseYear_ShouldReturnSingleElementList_WhenOnlyOneMovieExists()
    {
        // Arrange
        var singleList = new List<Movie> { new Movie { Title = "Solo", ReleaseYear = 2021 } };

        // Act
        var result = SortService.MergeSortByReleaseYear(singleList).ToList();

        // Assert
        Assert.Single(result);
        Assert.Equal("Solo", result[0].Title);
    }
}