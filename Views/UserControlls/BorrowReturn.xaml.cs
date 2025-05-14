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

        var user = _userRepository.GetUserById(userId);

        string result = _borrowReturnService.BorrowMovie(movieId, user!);
        MessageBox.Show(result);
        RefreshBorrowRecords();
    }

    private void ReturnButton_Click(object sender, RoutedEventArgs e)
    {
        string userId = UserIdBox.Input.Text.Trim();
        string movieId = MovieIdBox.Input.Text.Trim();

        var user = _userRepository.GetUserById(userId);

        string result = _borrowReturnService.ReturnMovie(movieId, user!);
        MessageBox.Show(result);
        RefreshBorrowRecords();
    }

    private void RefreshButton_Click(object sender, RoutedEventArgs e)
    {
        RefreshBorrowRecords();
    }
}