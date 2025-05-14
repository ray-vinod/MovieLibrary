using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using MovieLibrary.Data;
using MovieLibrary.Models;
using MovieLibrary.Services;

namespace MovieLibrary.Views.UserControlls;

public partial class MovieManagement : UserControl
{
    private MovieRepository _movieRepository;

    public MovieManagement()
    {
        InitializeComponent();

        _movieRepository = Repository.Instance.MovieRepo;

        RefreshDataGrid();
    }

    private void RefreshDataGrid()
    {
        var movies = _movieRepository.GetAllMovies().ToList();
        MoviesDataGrid.ItemsSource = movies;

        if (movies.Count == 0)
        {
            NotifierService.Instance.UpdateStatus($"There is no movie");
        }
    }

    private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (e.NewSize.Width < 600)
        {
            TitleSearchPanel.Orientation = Orientation.Vertical;
            TitleSearchPanel.HorizontalAlignment = HorizontalAlignment.Center;
        }
        else
        {
            TitleSearchPanel.Orientation = Orientation.Horizontal;
        }
    }

    // Linear search by title
    // Binary search by ID (ensure sorted first)
    private void TitleSearchButton_Click(object sender, RoutedEventArgs e)
    {
        string searchTitle = TitleSearchBox.Input.Text.Trim();
        string searchId = IdSearchBox.Input.Text.Trim();

        IdSearchBox.Input.Text="";
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

    // Bubble sort by title
    private void SortTitle_Click(object sender, RoutedEventArgs e)
    {
    }

    // Merge sort by ReleaseYear
    private void SortReleaseYear_Click(object sender, RoutedEventArgs e)
    {
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

            NotifierService.Instance.UpdateStatus($"{newMovie.Id}");

            _movieRepository.AddMovie(newMovie);

            RefreshDataGrid();

            MoviesDataGrid.IsReadOnly = false;
            MoviesDataGrid.CanUserAddRows = false;

            // Scroll to the new row and focus on it (important for the visual tree)
            MoviesDataGrid.ScrollIntoView(newMovie);

            Dispatcher.BeginInvoke(new Action(() =>
            {
                var movies = _movieRepository.GetAllMovies();
                var lastRowIndex = movies.Count() - 1;

                var lastRow = MoviesDataGrid.ItemContainerGenerator.ContainerFromIndex(lastRowIndex) as DataGridRow;

                if (lastRow != null)
                {
                    MoviesDataGrid.CurrentCell = new DataGridCellInfo(newMovie, MoviesDataGrid.Columns[1]);
                    MoviesDataGrid.BeginEdit();

                    var editButton = Helpers.DataGrid.FindButtonInRow(lastRow, "EditButton");
                    var deleteButton = Helpers.DataGrid.FindButtonInRow(lastRow, "DeleteButton");

                    if (editButton != null)
                    {
                        editButton.Content = "Save";
                    }

                    if (deleteButton != null)
                    {
                        deleteButton.Content = "Cancel";
                    }
                }
            }), DispatcherPriority.Background);

            // NotifierService.Instance.UpdateStatus($"New movie '{newMovie.Title}' is added.");
        }
        catch (Exception ex)
        {
            NotifierService.Instance.UpdateStatus($"An error occurred while adding the movie: {ex.Message}");
            MessageBox.Show($"An error occurred while adding the movie: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
        catch (Exception ex)
        {
            NotifierService.Instance.UpdateStatus($"An error occurred while deleting the movie '{movie.Title}'.");
            MessageBox.Show($"An error occurred while deleting the movie: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

    }
}