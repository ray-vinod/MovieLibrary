using MovieLibrary.Models;
using MovieLibrary.Services;

namespace MovieLibrary.Test.Services;

public class ImportExportServiceTests : IDisposable
{
    private readonly string _testFilePath = Path.Combine(Path.GetTempPath(), "test_movies.json");

    [Fact]
    public void ExportMovies_ShouldCreateFile_WithCorrectContent()
    {
        // Arrange
        var movies = new List<Movie>
        {
            new Movie { Id = "M001", Title = "Export Test", Director = "Dir A", Genre = "Genre A", ReleaseYear = 2022, IsAvailable = true },
            new Movie { Id = "M002", Title = "Another Movie", Director = "Dir B", Genre = "Genre B", ReleaseYear = 2023, IsAvailable = false }
        };

        // Act
        ImportExportService.ExportMovies(movies, _testFilePath);

        // Assert
        Assert.True(File.Exists(_testFilePath));
        var content = File.ReadAllText(_testFilePath);
        Assert.Contains("Export Test", content);
        Assert.Contains("Another Movie", content);
    }

    [Fact]
    public void ImportMovies_ShouldReadFile_AndReturnMovieList()
    {
        // Arrange
        var json = """
        [
          {
            "id": "M101",
            "title": "Import Test",
            "director": "Import Dir",
            "genre": "Drama",
            "releaseYear": 2020,
            "isAvailable": true
          }
        ]
        """;

        File.WriteAllText(_testFilePath, json);

        // Act
        var result = ImportExportService.ImportMovies(_testFilePath).ToList();

        // Assert
        Assert.Single(result);
        Assert.Equal("M101", result[0].Id);
        Assert.Equal("Import Test", result[0].Title);
        Assert.Equal("Drama", result[0].Genre);
        Assert.True(result[0].IsAvailable);
    }

    [Fact]
    public void ImportMovies_ShouldReturnEmpty_WhenFileDoesNotExist()
    {
        // Arrange
        var nonExistentPath = Path.Combine(Path.GetTempPath(), "nonexistent_file.json");

        // Act
        var result = ImportExportService.ImportMovies(nonExistentPath);

        // Assert
        Assert.Empty(result);
    }

    public void Dispose()
    {
        if (File.Exists(_testFilePath))
        {
            File.Delete(_testFilePath);
        }
    }
}
