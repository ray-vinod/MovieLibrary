using System.Windows;
using System.Windows.Controls;

using Microsoft.Win32;

using MovieLibrary.Data;
using MovieLibrary.Models;
using MovieLibrary.Services;

namespace MovieLibrary.Views.UserControls;

public partial class ImportExport : UserControl
{
	private readonly MovieRepository _movieRepository;
	private readonly UserRepository _userRepository;

	public ImportExport()
	{
		InitializeComponent();
		_movieRepository = Repository.Instance.MovieRepo;
		_userRepository = Repository.Instance.UserRepo;
	}

	private void ImportData<T>(
	Func<T, bool> isDuplicate,
	Action<T> addEntity,
	string entityType,
	Func<T, bool>? isValid = null
) where T : class
	{
		var openFileDialog = new OpenFileDialog { Filter = "JSON File|*.json" };
		if (openFileDialog.ShowDialog() != true) return;

		try
		{
			var data = ImportExportService.ImportData<T>(openFileDialog.FileName);
			int successCount = 0;
			int skippedCount = 0;

			foreach (var item in data)
			{
				if ((isValid != null && !isValid(item)) || isDuplicate(item))
				{
					skippedCount++;
					continue;
				}

				addEntity(item);
				successCount++;
			}

			NotifierService.Instance.UpdateStatus(
				$"{entityType}s Imported: {successCount}, Skipped: {skippedCount} (invalid or duplicate)");
		}
		catch (Exception ex)
		{
			NotifierService.Instance.UpdateStatus($"Failed to import {entityType.ToLower()}s.");
			MessageBox.Show(ex.Message);
		}
	}


	private void ExportData<T>(IEnumerable<T> data, string entityType)
	{
		var saveFileDialog = new SaveFileDialog { Filter = "JSON File|*.json" };
		if (saveFileDialog.ShowDialog() != true) return;

		try
		{
			ImportExportService.ExportData(data, saveFileDialog.FileName);
			NotifierService.Instance.UpdateStatus($"{entityType}s exported successfully.");
		}
		catch (Exception)
		{
			NotifierService.Instance.UpdateStatus($"Failed to export {entityType.ToLower()}s.");
		}
	}

	private void ImportMovieButton_Click(object sender, RoutedEventArgs e)
	{
		ImportData<Movie>(
			movie => _movieRepository.GetMovieById(movie.Id!) != null ||
					 _movieRepository.GetAllMovies().Any(m => m.Title!.Equals(movie.Title, StringComparison.OrdinalIgnoreCase)),
			_movieRepository.AddMovie,
			"Movie",
			movie => !string.IsNullOrWhiteSpace(movie.Title) &&
					 movie.ReleaseYear >= 1900 &&
					 movie.ReleaseYear <= DateTime.Now.Year
		);
	}


	private void ExportMovieButton_Click(object sender, System.Windows.RoutedEventArgs e)
	{
		ExportData(_movieRepository.GetAllMovies(), "Movie");
	}

	private void ImportUserButton_Click(object sender, RoutedEventArgs e)
	{
		ImportData<User>(
			user => _userRepository.GetUserById(user.Id!) != null,
			_userRepository.AddUser,
			"User",
			user => !string.IsNullOrWhiteSpace(user.Id) && !string.IsNullOrWhiteSpace(user.Name)
		);
	}


	private void ExportUserButton_Click(object sender, System.Windows.RoutedEventArgs e)
	{
		ExportData(_userRepository.GetAllUsers(), "User");
	}
}