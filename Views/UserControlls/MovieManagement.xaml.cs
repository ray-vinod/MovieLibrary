using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using MovieLibrary.Data;
using MovieLibrary.Models;

namespace MovieLibrary.Views.UserControlls;

public partial class MovieManagement : UserControl
{
    private MovieRepository? _movieRepository;

    public MovieManagement()
    {
        InitializeComponent();

        _movieRepository = Repository.Instance.MovieRepo;

        RefreshDataGrid();
    }

    private void RefreshDataGrid()
    {
        MoviesDataGrid.ItemsSource = _movieRepository?.GetAllMovies().ToList();
    }

    // Linear search by title
    private void TitleSearchButton_Click(object sender, RoutedEventArgs e)
    {
    }

    // Binary search by ID (ensure sorted first)
    private void IdSearchButton_Click(object sender, RoutedEventArgs e)
    {
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

                        if (editButton != null)
                        {
                            editButton.Content = "Edit";
                            MoviesDataGrid.IsReadOnly = true;
                        }
                    }
                }), DispatcherPriority.Background);
            }
        }
    }

    private void Add_Click(object sender, RoutedEventArgs e)
    {
        var newMovie = new Movie
        {
            Id = Repository.Instance.GenerateNewMovieId(),
            Title = "",
            Genre = "",
            Director = "",
            ReleaseYear = 0,
            IsAvailable = true,
        };

        _movieRepository?.AddMovie(newMovie);

        RefreshDataGrid();

        MoviesDataGrid.IsReadOnly = false;
        MoviesDataGrid.CanUserAddRows = false;

        // Scroll to the new row and focus on it
        MoviesDataGrid.ScrollIntoView(newMovie);
        Dispatcher.BeginInvoke(new Action(() =>
        {
            var movies = _movieRepository?.GetAllMovies();
            var lastRowIndex = movies!.Count() - 1;
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
        _movieRepository?.DeleteMovie(movie.Id!);

        RefreshDataGrid();
    }
}