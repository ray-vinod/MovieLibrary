using MovieLibrary.Models;
using MovieLibrary.Services;

namespace MovieLibrary.Test.Services;
public class ImportExportServiceTests
{
	private string GetTempFilePath() => Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".json");

	[Fact]
	public void ExportAndImportMovies_ShouldPreserveData()
	{
		// Arrange
		var movies = new List<Movie>
		{
			new Movie { Id = "M001", Title = "Inception", Director = "Christopher Nolan", Genre = "Sci-Fi", ReleaseYear = 2010, IsAvailable = true },
			new Movie { Id = "M002", Title = "The Matrix", Director = "Wachowskis", Genre = "Action", ReleaseYear = 1999, IsAvailable = false }
		};
		var filePath = GetTempFilePath();

		try
		{
			// Act
			ImportExportService.ExportData(movies, filePath);
			var importedMovies = ImportExportService.ImportData<Movie>(filePath).ToList();

			// Assert
			Assert.Equal(movies.Count, importedMovies.Count);
			for (int i = 0; i < movies.Count; i++)
			{
				Assert.Equal(movies[i].Id, importedMovies[i].Id);
				Assert.Equal(movies[i].Title, importedMovies[i].Title);
				Assert.Equal(movies[i].Director, importedMovies[i].Director);
				Assert.Equal(movies[i].Genre, importedMovies[i].Genre);
				Assert.Equal(movies[i].ReleaseYear, importedMovies[i].ReleaseYear);
				Assert.Equal(movies[i].IsAvailable, importedMovies[i].IsAvailable);
			}
		}
		finally
		{
			if (File.Exists(filePath)) File.Delete(filePath);
		}
	}

	[Fact]
	public void ExportAndImportUsers_ShouldPreserveData()
	{
		// Arrange
		var users = new List<User>
		{
			new User { Id = "U001", Name = "Bibek" },
			new User { Id = "U002", Name = "Alice" }
		};
		var filePath = GetTempFilePath();

		try
		{
			// Act
			ImportExportService.ExportData(users, filePath);
			var importedUsers = ImportExportService.ImportData<User>(filePath).ToList();

			// Assert
			Assert.Equal(users.Count, importedUsers.Count);
			for (int i = 0; i < users.Count; i++)
			{
				Assert.Equal(users[i].Id, importedUsers[i].Id);
				Assert.Equal(users[i].Name, importedUsers[i].Name);
			}
		}
		finally
		{
			if (File.Exists(filePath)) File.Delete(filePath);
		}
	}

	[Fact]
	public void ImportData_FileNotFound_ShouldReturnEmptyCollection()
	{
		// Arrange
		var filePath = Path.Combine(Path.GetTempPath(), "nonexistentfile.json");

		// Act
		var result = ImportExportService.ImportData<Movie>(filePath);

		// Assert
		Assert.Empty(result);
	}
}
