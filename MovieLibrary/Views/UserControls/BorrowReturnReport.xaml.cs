using MovieLibrary.Data;
using MovieLibrary.Models;
using MovieLibrary.Services;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MovieLibrary.Views.UserControls;
public partial class BorrowReturnReport : UserControl
{
	private readonly BorrowReturnService _borrowReturnService;
	private readonly MovieRepository _movieRepository;
	private readonly UserRepository _userRepository;

	private readonly BorrowRecordRepository _borrowRepo;
	private int _currentPage = 1;
	private int _pageSize = 8;
	private int _totalPages = 1;

	private User? _user = null;
	private Movie? _movie = null;

	public BorrowReturnReport()
	{
		InitializeComponent();

		_movieRepository = Repository.Instance.MovieRepo;
		_userRepository = Repository.Instance.UserRepo;
		_borrowReturnService = new BorrowReturnService(_movieRepository);

		_borrowRepo = Repository.Instance.RecordRepo;

		MovieDataGrid.Visibility = Visibility.Hidden;
		BorrowRecordsDataGrid.Visibility = Visibility.Visible;

		MovieTitleBox.IsEnabled = false;

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
		BorrowRecordsDataGrid.ItemsSource = null;
		BorrowRecordsDataGrid.ItemsSource = Repository.Instance.BorrowRecords.ToList();
		BorrowRecordsDataGrid.Visibility = Visibility.Visible;
		_user = null;
		_movie = null;
		FormReset();
	}


	private void ProcessButton_Click(object sender, RoutedEventArgs e)
	{
		string userName = UserNameBox.Input.Text.Trim();

		if (string.IsNullOrWhiteSpace(userName))
		{
			NotifierService.Instance.UpdateStatus("User name is required");
			MovieDataGrid.Visibility = Visibility.Hidden;
			MovieDataGrid.ItemsSource = null;
			RefreshBorrowRecords();
			
			return;
		}

		if (_user == null)
		{
			NotifierService.Instance.UpdateStatus($"You are not a member!");

			MovieDataGrid.Visibility = Visibility.Hidden;
			MovieDataGrid.ItemsSource = null;
			RefreshBorrowRecords();

			return;
		}

		string movieTitle = MovieTitleBox.Input.Text.Trim();

		if (string.IsNullOrWhiteSpace(movieTitle))
		{
			NotifierService.Instance.UpdateStatus("Movie's name is requied.");

			MovieDataGrid.Visibility = Visibility.Hidden;
			MovieDataGrid.ItemsSource = null;
			RefreshBorrowRecords();

			return;
		}

		_movie = _movieRepository.GetAllMovies()
			.FirstOrDefault(m => string.Equals(m.Title, movieTitle, StringComparison.OrdinalIgnoreCase));

		if (_movie == null)
		{
			NotifierService.Instance.UpdateStatus($"The requested movie '{movieTitle}' is not available.");

			MovieDataGrid.Visibility = Visibility.Hidden;
			MovieDataGrid.ItemsSource = null;
			RefreshBorrowRecords();

			return;
		}

		// for the return -> if has borrowed then show in return
		var record = Repository.Instance.BorrowRecords
			.FirstOrDefault(x => string.Equals(x.UserId, _user.Id) && string.Equals(x.MovieId, _movie.Id) && x.IsActive);

		if (record != null)
		{
			var records = new List<BorrowRecord>
			{
				record
			};

			BorrowRecordsDataGrid.ItemsSource = null;
			BorrowRecordsDataGrid.ItemsSource = records;
			ActionHeader.Visibility = Visibility.Visible;

			FormReset();
			NotifierService.Instance.UpdateStatus($"{record.UserName} has borrowed {record.MovieTitle}");
			return;
		}

		var movies = new List<Movie>()
		{
			_movie
		};

		MovieDataGrid.ItemsSource = null;
		MovieDataGrid.ItemsSource = movies;

		MovieDataGrid.Visibility = Visibility.Visible;
		BorrowRecordsDataGrid.Visibility = Visibility.Hidden;

		FormReset();
	}

	private void FormReset()
	{
		UserNameBox.BorderBrush = new SolidColorBrush(Color.FromRgb(171, 173, 179));
		UserNameBox.BorderThickness = new Thickness(0.25);


		UserNameBox.Input.Text = "";
		MovieTitleBox.Input.Text = "";
	}

	private void ReturnButton_Click(object sender, RoutedEventArgs e)
	{
		string result = _borrowReturnService.ReturnMovie(_movie!.Id!, _user!);
		NotifierService.Instance.UpdateStatus(result);

		MovieDataGrid.ItemsSource = null;
		MovieDataGrid.Visibility = Visibility.Hidden;
		BorrowRecordsDataGrid.Visibility = Visibility.Visible;
		ActionHeader.Visibility = Visibility.Hidden;

		RefreshBorrowRecords();
	}

	private void BorrowButton_Click(object sender, RoutedEventArgs e)
	{
		// borrowing movie
		string result = _borrowReturnService.BorrowMovie(_movie!.Id!, _user!);
		NotifierService.Instance.UpdateStatus(result);

		MovieDataGrid.ItemsSource = null;
		MovieDataGrid.Visibility = Visibility.Hidden;
		BorrowRecordsDataGrid.Visibility = Visibility.Visible;

		RefreshBorrowRecords();
	}

	// Find Valid User
	private void UserNameBox_LostFocus(object sender, RoutedEventArgs e)
	{
		string userName = UserNameBox.Input.Text.Trim();

		if (string.IsNullOrWhiteSpace(userName))
		{
			NotifierService.Instance.UpdateStatus("User name is required");
			return;
		}

		_user = _userRepository.GetAllUsers()
			.FirstOrDefault(u => string.Equals(u.Name, userName, StringComparison.OrdinalIgnoreCase));

		if (_user == null)
		{
			MovieDataGrid.ItemsSource = null;
			MovieDataGrid.Visibility = Visibility.Hidden;
			BorrowRecordsDataGrid.Visibility = Visibility.Visible;

			MovieTitleBox.IsEnabled = false;
			UserNameBox.BorderBrush = Brushes.IndianRed;
			UserNameBox.BorderThickness = new Thickness(2);
			NotifierService.Instance.UpdateStatus($"{userName} is not a member!");
			return;
		}

		UserNameBox.BorderBrush = Brushes.LightGreen;
		UserNameBox.BorderThickness = new Thickness(2);
		MovieTitleBox.IsEnabled = true;

		NotifierService.Instance.UpdateStatus($"Welcome, {_user.Name} !");
	}
}