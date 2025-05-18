using System.Windows;
using System.Windows.Controls;
using MovieLibrary.Data;
using MovieLibrary.Services;

namespace MovieLibrary.Views.UserControls;

public partial class BorrowReturn : UserControl
{
    private BorrowReturnService _borrowReturnService;
    private MovieRepository _movieRepository;
    private UserRepository _userRepository;

	private BorrowRecordRepository _borrowRepo;
	private int _currentPage = 1;
	private int _pageSize = 8;
	private int _totalPages = 1;

	public BorrowReturn()
    {
        InitializeComponent();
        _movieRepository = Repository.Instance.MovieRepo;
        _userRepository = Repository.Instance.UserRepo;
        _borrowReturnService = new BorrowReturnService(_movieRepository);

        _borrowRepo = Repository.Instance.RecordRepo;
        UpdatePagination();
        RefreshBorrowRecords();
    }

	private void UpdatePagination()
	{
		var allRecords = _borrowRepo.GetAllBorrowRecord().ToList();
		_totalPages = (int)Math.Ceiling(allRecords.Count / (double)_pageSize);

		if (_currentPage > _totalPages) _currentPage = _totalPages == 0 ? 1 : _totalPages;

		var pagedRecords = allRecords
			.Skip((_currentPage - 1) * _pageSize)
			.Take(_pageSize)
			.ToList();

		BorrowRecordsDataGrid.ItemsSource = pagedRecords;

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

	private void RefreshButton_Click(object sender, RoutedEventArgs e)
	{
		_currentPage = 1;
		UpdatePagination();
	}

    private void RefreshBorrowRecords()
    {
        BorrowRecordsDataGrid.ItemsSource = Repository.Instance.BorrowRecords.ToList();
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
}