using MovieLibrary.Data;
using MovieLibrary.Models;
using MovieLibrary.Services;

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace MovieLibrary.Views.UserControls;

public partial class MovieManagement : UserControl
{
	private MovieRepository _movieRepository;

	private int _currentPage = 1;
	private int _pageSize = 8;
	private int _totalPages = 1;

	public MovieManagement()
	{
		InitializeComponent();

		_movieRepository = Repository.Instance.MovieRepo;

		UpdatePagination();
	}

	private void UpdatePagination()
	{
		var allMovies = _movieRepository.GetAllMovies().ToList();
		_totalPages = (int)Math.Ceiling(allMovies.Count / (double)_pageSize);

		if (_currentPage > _totalPages) _currentPage = _totalPages == 0 ? 1 : _totalPages;

		var pagedMovies = allMovies
			.Skip((_currentPage - 1) * _pageSize)
			.Take(_pageSize)
			.ToList();

		MoviesDataGrid.ItemsSource = pagedMovies;

		PaginationTextBlock.Text = $"Page {_currentPage} of {_totalPages}";
		PrevPageButton.IsEnabled = _currentPage > 1;
		NextPageButton.IsEnabled = _currentPage < _totalPages;
	}

	private void PrevPageButton_Click(object sender, RoutedEventArgs e)
	{
		if (_currentPage > 1)
		{
			_currentPage--;
			UpdatePagination();
		}
	}

	private void NextPageButton_Click(object sender, RoutedEventArgs e)
	{
		if (_currentPage < _totalPages)
		{
			_currentPage++;
			UpdatePagination();
		}
	}

	private void RefreshDataGrid()
	{
		UpdatePagination();
		var allMovies = _movieRepository.GetAllMovies().ToList();
		if (allMovies.Count == 0)
		{
			NotifierService.Instance.UpdateStatus($"There is no movie");
		}
	}

	// Linear search by title
	// Binary search by ID (ensure sorted first)
	private void TitleSearchButton_Click(object sender, RoutedEventArgs e)
	{
		string searchTitle = TitleSearchBox.Input.Text.Trim();
		string searchId = IdSearchBox.Input.Text.Trim();

		IdSearchBox.Input.Text = "";
		TitleSearchBox.Input.Text = "";

		IEnumerable<Movie> movies = _movieRepository.GetAllMovies();
		List<Movie> searchedMovies = new();

		if (!string.IsNullOrWhiteSpace(searchTitle))
		{
			searchedMovies = SearchService.SearchMoviesByTitle(movies, searchTitle).ToList();

			NotifierService.Instance.UpdateStatus("Searching movie by TITLE using Linear method.");
		}

		if (!string.IsNullOrWhiteSpace(searchId))
		{
			var searchedMovie = SearchService.SearchMovieById(movies, searchId);
			searchedMovies.Add(searchedMovie!);

			NotifierService.Instance.UpdateStatus("Searching movie by ID using Binary method.");
		}

		MoviesDataGrid.ItemsSource = searchedMovies;
	}

	private void RefreshButton_Click(object sender, RoutedEventArgs e)
	{
		RefreshDataGrid();
	}

	private void Edit_Click(object sender, RoutedEventArgs e)
	{
		var button = sender as Button;
		var movie = button?.Tag as Movie;

		if (movie != null)
		{
			if (button!.Content.ToString() == "Edit")
			{
				button.Content = "Save";
				MoviesDataGrid.IsReadOnly = false;
				MoviesDataGrid.Columns[0].IsReadOnly = true;
				MoviesDataGrid.CurrentCell = new DataGridCellInfo(movie, MoviesDataGrid.Columns[1]);
				MoviesDataGrid.BeginEdit();
			}
			else if (button.Content.ToString() == "Save")
			{
				MoviesDataGrid.CommitEdit();
				MoviesDataGrid.IsReadOnly = true;
				button.Content = "Edit";
			}
		}
	}

	private void Delete_Click(object sender, RoutedEventArgs e)
	{
		var button = sender as Button;
		var movie = button?.Tag as Movie;
		if (movie != null)
		{
			if (button!.Content.ToString() == "Delete")
			{
				if (MessageBox.Show($"Are you sure you want to delete \"{movie.Title}\"?", "Confirm Delete", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
				{
					MovieRemove(movie);
				}
			}
			else if (button.Content.ToString() == "Cancel")
			{
				MovieRemove(movie);
			}
		}
	}

	private void MoviesDataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
	{
		if (e.EditAction == DataGridEditAction.Commit)
		{
			var row = e.Row;
			var movie = row.Item as Movie;

			if (movie != null)
			{
				Dispatcher.BeginInvoke(new Action(() =>
				{
					var rowIndex = MoviesDataGrid.Items.IndexOf(movie);
					var selectedRow = MoviesDataGrid.ItemContainerGenerator.ContainerFromIndex(rowIndex) as DataGridRow;

					if (selectedRow != null)
					{
						var editButton = Helpers.DataGrid.FindButtonInRow(selectedRow, "EditButton");
						var deleteButton = Helpers.DataGrid.FindButtonInRow(selectedRow, "DeleteButton");

						if (deleteButton != null && deleteButton.Content.Equals("Delete"))
						{
							NotifierService.Instance.UpdateStatus($"The movie '{movie.Title}' is edited.");
						}
						else
						{
							NotifierService.Instance.UpdateStatus($"New movie '{movie.Title}' is added.");
						}

						if (editButton != null)
						{
							editButton.Content = "Edit";
						}

						if (deleteButton != null)
						{
							deleteButton.Content = "Delete";
						}

						MoviesDataGrid.IsReadOnly = true;
					}
				}), DispatcherPriority.Background);
			}
		}
	}

	private void MoviesDataGrid_Sorting(object sender, DataGridSortingEventArgs e)
	{
		e.Handled = true; // Prevent default sort

		var allMovies = _movieRepository.GetAllMovies().ToList();
		IEnumerable<Movie> sortedMovies = allMovies;

		if (e.Column.Header.ToString() == "Title")
		{
			sortedMovies = Services.SortService.BubbleSortByTitle(allMovies);
			NotifierService.Instance.UpdateStatus("Sorted by Title using Bubble Sort.");
		}
		else if (e.Column.Header.ToString() == "ReleaseYear")
		{
			sortedMovies = Services.SortService.MergeSortByReleaseYear(allMovies);
			NotifierService.Instance.UpdateStatus("Sorted by Release Year using Merge Sort.");
		}
		else
		{
			return;
		}

		// Toggle sort direction
		if (e.Column.SortDirection == null || e.Column.SortDirection == ListSortDirection.Descending)
		{
			e.Column.SortDirection = ListSortDirection.Ascending;
		}
		else
		{
			sortedMovies = sortedMovies.Reverse();
			e.Column.SortDirection = ListSortDirection.Descending;
		}

		_currentPage = 1;
		MoviesDataGrid.ItemsSource = sortedMovies
			.Skip((_currentPage - 1) * _pageSize)
			.Take(_pageSize)
			.ToList();

		// FormReset sort direction from other columns
		foreach (var col in MoviesDataGrid.Columns)
		{
			if (col != e.Column)
				col.SortDirection = null;
		}
	}

	private void Add_Click(object sender, RoutedEventArgs e)
	{
		try
		{
			var newMovie = new Movie
			{
				Id = Repository.Instance.GenerateNewMovieId(),
				Title = "Title",
				Genre = "Genre",
				Director = "UnKnown",
				ReleaseYear = DateTime.Now.Year,
				IsAvailable = true,
			};

			_movieRepository.AddMovie(newMovie);

			// Go to the last page
			var allMovies = _movieRepository.GetAllMovies().ToList();
			_totalPages = (int)Math.Ceiling(allMovies.Count / (double)_pageSize);
			_currentPage = _totalPages;

			UpdatePagination();

			MoviesDataGrid.IsReadOnly = false;
			MoviesDataGrid.CanUserAddRows = false;

			// Scroll into view of the new movie (should now be on the last page)
			Dispatcher.BeginInvoke(new Action(() =>
			{
				var pagedMovies = MoviesDataGrid.ItemsSource as List<Movie>;
				var index = pagedMovies?.IndexOf(newMovie) ?? -1;

				if (index >= 0)
				{
					var lastRow = MoviesDataGrid.ItemContainerGenerator.ContainerFromIndex(index) as DataGridRow;

					MoviesDataGrid.ScrollIntoView(newMovie);

					MoviesDataGrid.CurrentCell = new DataGridCellInfo(newMovie, MoviesDataGrid.Columns[1]);
					MoviesDataGrid.BeginEdit();

					if (lastRow != null)
					{
						var editButton = Helpers.DataGrid.FindButtonInRow(lastRow, "EditButton");
						var deleteButton = Helpers.DataGrid.FindButtonInRow(lastRow, "DeleteButton");

						if (editButton != null)
							editButton.Content = "Save";
						if (deleteButton != null)
							deleteButton.Content = "Cancel";
					}
				}

			}), DispatcherPriority.Background);
		}
		catch (Exception)
		{
			NotifierService.Instance.UpdateStatus($"An error occurred while adding the movie");
		}
	}

	private void MoviesDataGrid_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.Key == Key.Escape)
		{
			var selectedMovie = MoviesDataGrid.SelectedItem as Movie;
			if (selectedMovie != null)
			{
				MovieRemove(selectedMovie);
			}
		}
	}

	private void MovieRemove(Movie movie)
	{
		try
		{
			_movieRepository.DeleteMovie(movie.Id!);

			NotifierService.Instance.UpdateStatus($"The movie '{movie.Title}' has been deleted.");

			RefreshDataGrid();
		}
		catch (Exception)
		{
			NotifierService.Instance.UpdateStatus($"An error occurred while deleting the movie '{movie.Title}'.");
		}
	}
}