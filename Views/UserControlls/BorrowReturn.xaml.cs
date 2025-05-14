using System.Windows;
using System.Windows.Controls;
using MovieLibrary.Data;
using MovieLibrary.Services;

namespace MovieLibrary.Views.UserControlls;

public partial class BorrowReturn : UserControl
{
    private BorrowReturnService _borrowReturnService;
    private MovieRepository _movieRepository;
    private UserRepository _userRepository;

    public BorrowReturn()
    {
        InitializeComponent();
        _movieRepository = Repository.Instance.MovieRepo;
        _userRepository = Repository.Instance.UserRepo;
        _borrowReturnService = new BorrowReturnService(_movieRepository);
        RefreshBorrowRecords();
    }

    private void RefreshBorrowRecords()
    {
        BorrowRecordsDataGrid.ItemsSource = _borrowReturnService.BorrowRecords.ToList();
    }

    private void BorrowButton_Click(object sender, RoutedEventArgs e)
    {
        string userId = UserIdBox.Input.Text.Trim();
        string movieId = MovieIdBox.Input.Text.Trim();

        if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(movieId))
        {
            NotifierService.Instance.UpdateStatus("There is incomplete information userId/movieId");
            return;
        }

        var user = _userRepository.GetAllUsers()
            .FirstOrDefault(u => string.Equals(u.Id, userId, StringComparison.OrdinalIgnoreCase));
        if (user == null)
        {
            NotifierService.Instance.UpdateStatus("User not found.");
            return;
        }

        var movie = _movieRepository.GetAllMovies()
            .FirstOrDefault(m => string.Equals(m.Id, movieId, StringComparison.OrdinalIgnoreCase));
        if (movie == null)
        {
            NotifierService.Instance.UpdateStatus("Movie not found.");
            return;
        }

        string result = _borrowReturnService.BorrowMovie(movie.Id!, user);
        Clear();

        NotifierService.Instance.UpdateStatus(result);
        RefreshBorrowRecords();
    }

    private void Clear()
    {
        UserIdBox.Input.Text = "";
        MovieIdBox.Input.Text = "";
    }

    private void ReturnButton_Click(object sender, RoutedEventArgs e)
    {
        string userId = UserIdBox.Input.Text.Trim();
        string movieId = MovieIdBox.Input.Text.Trim();

        if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(movieId))
        {
            NotifierService.Instance.UpdateStatus("There is incomplete information userId/movieId");
            return;
        }

        var user = _userRepository.GetAllUsers()
            .FirstOrDefault(u => string.Equals(u.Id, userId, StringComparison.OrdinalIgnoreCase));
        if (user == null)
        {
            NotifierService.Instance.UpdateStatus("User not found.");
            return;
        }

        var movie = _movieRepository.GetAllMovies()
            .FirstOrDefault(m => string.Equals(m.Id, movieId, StringComparison.OrdinalIgnoreCase));
        if (movie == null)
        {
            NotifierService.Instance.UpdateStatus("Movie not found.");
            return;
        }

        string result = _borrowReturnService.ReturnMovie(movie.Id!, user);

        Clear();

        NotifierService.Instance.UpdateStatus(result);
        RefreshBorrowRecords();
    }

    private void RefreshButton_Click(object sender, RoutedEventArgs e)
    {
        RefreshBorrowRecords();
    }
}