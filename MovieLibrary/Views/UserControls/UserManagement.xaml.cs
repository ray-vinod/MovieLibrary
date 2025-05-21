using MovieLibrary.Data;
using MovieLibrary.Helpers;
using MovieLibrary.Models;
using MovieLibrary.Services;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace MovieLibrary.Views.UserControls;

public partial class UserManagement : UserControl
{
	private readonly UserRepository _repository;
	private List<User> _allUsers = [];
	private User? _editingUser = null;

	private int _currentPage = 1;
	private int _pageSize = 5;
	private int _totalPages = 1;

	private bool _awaitingDuplicateConfirmation = false;


	public UserManagement()
	{
		InitializeComponent();
		_repository = Repository.Instance.UserRepo;
		Loaded += (s, e) => LoadUsers();
	}

	private void NameBox_TextChanged(object sender, TextChangedEventArgs e)
	{
		if (_awaitingDuplicateConfirmation)
		{
			AddOrSaveButton.Content = CreateButtonContent("Add User", "/assets/add.png");
		}

		_awaitingDuplicateConfirmation = false;
	}

	public void PreviousPage_Click(object sender, RoutedEventArgs e)
	{
		if (_currentPage > 1)
		{
			_currentPage--;
			DisplayPage(_currentPage);
		}
	}

	public void NextPage_Click(object sender, RoutedEventArgs e)
	{
		if (_currentPage < _totalPages)
		{
			_currentPage++;
			DisplayPage(_currentPage);
		}
	}


	private void DisplayPage(int pageNumber)
	{
		var itemsToShow = _allUsers
			.Skip((pageNumber - 1) * _pageSize)
			.Take(_pageSize)
			.ToList();

		UserListView.ItemsSource = itemsToShow;

		PageInfoText.Text = $"Page {pageNumber} of {_totalPages} (Total: {_allUsers.Count})";
		PreviousButton.IsEnabled = pageNumber > 1;
		NextButton.IsEnabled = pageNumber < _totalPages;
	}


	private void LoadUsers(bool goToLastPage = false)
	{
		_allUsers = _repository.GetAllUsers()
			.OrderBy(x => x.Id)
			.ToList();
		_totalPages = (_allUsers.Count + _pageSize - 1) / _pageSize;

		if (_currentPage > _totalPages)
		{
			_currentPage = _totalPages == 0 ? 1 : _totalPages;
		}

		if (goToLastPage)
		{
			_currentPage = _totalPages;
		}

		DisplayPage(_currentPage);
	}

	private int FindPageOfUser(User user)
	{
		int index = _allUsers.FindIndex(u => u.Id == user.Id);
		return index >= 0 ? (index / _pageSize) + 1 : 1;
	}


	private void AddOrSaveButton_Click(object sender, RoutedEventArgs e)
	{
		var name = NameBox.Input.Text.Trim();
		var button = sender as Button;

		if (string.IsNullOrWhiteSpace(name))
		{
			NotifierService.Instance.UpdateStatus("ID and Name are required.");
			return;
		}

		if (_editingUser == null) // add new user
		{
			// inform dubplication
			var existingUsers = _repository.GetAllUsers().Where(u => u.Name!.Equals(name, StringComparison.OrdinalIgnoreCase)).ToList();

			if (existingUsers.Any() && !_awaitingDuplicateConfirmation)
			{
				var firstMatch = existingUsers.First();

				int targetPage = FindPageOfUser(firstMatch);
				if (_currentPage != targetPage)
				{
					_currentPage = targetPage;
					LoadUsers();
					Dispatcher.BeginInvoke(new Action(() =>
					{
						UserListView.SelectedItem = firstMatch;
						UserListView.ScrollIntoView(firstMatch);
					}), DispatcherPriority.Background);
				}
				else
				{
					UserListView.SelectedItem = firstMatch;
					UserListView.ScrollIntoView(firstMatch);
				}

				NotifierService.Instance.UpdateStatus("User with this name already exists. Confirm for adding new one");

				// change button text
				if (button != null)
				{
					AddOrSaveButton.Content = CreateButtonContent("Confirm", "/assets/add.png");
				}

				_awaitingDuplicateConfirmation = true;
				return;
			}

			// add new
			try
			{
				var user = new User
				{
					Id = Repository.Instance.GenerateNewUserId(),
					Name = name
				};

				_repository.AddUser(user);
				NotifierService.Instance.UpdateStatus("User added.");
				LoadUsers(goToLastPage: true); // Jump to last page
			}
			catch (Exception ex)
			{
				NotifierService.Instance.UpdateStatus($"Error adding user: {ex.Message}");
			}
		}
		else // edit user
		{
			try
			{
				_editingUser.Name = name;
				_repository.UpdateUser(_editingUser);
				NotifierService.Instance.UpdateStatus("User updated.");
				LoadUsers();
			}
			catch (Exception ex)
			{
				NotifierService.Instance.UpdateStatus($"Error updating user: {ex.Message}");
			}
		}

		ClearForm();
		MainScrollViewer.ScrollToTop();
	}


	private void EditText_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		if (sender is TextBlock textBlock &&
			textBlock.DataContext is User user)
		{
			EditCommand.Execute(user);
		}
	}

	private void DeleteText_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		if (sender is TextBlock textBlock &&
			textBlock.DataContext is User user)
		{
			DeleteCommand.Execute(user);
		}
	}

	private void ClearForm()
	{
		_editingUser = null;
		NameBox.Input.Text = "";
		FormHeading.Text = "New User";
		AddOrSaveButton.Content = "Add User";
		AddOrSaveButton.Content = CreateButtonContent("Add User", "/assets/add.png");
	}

	private RelayCommand? _editCommand;
	public RelayCommand EditCommand => _editCommand ??= new RelayCommand(EditCommand_Execute);

	private void EditCommand_Execute(object? sender, object? parameter)
	{
		if (parameter is User user)
		{
			_editingUser = user;
			NameBox.Input.Text = user.Name;
			FormHeading.Text = "Edit User";
			AddOrSaveButton.Content = "Save Changes";
			AddOrSaveButton.Width = 145;
			AddOrSaveButton.Content = CreateButtonContent("Save Changes", "/assets/add.png");
		}
	}

	private static StackPanel CreateButtonContent(string text, string iconPath)
	{
		var stack = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Center };
		var img = new Image
		{
			Source = new System.Windows.Media.Imaging.BitmapImage(new System.Uri(iconPath, System.UriKind.Relative)),
			Width = 16,
			Height = 16,
			Margin = new Thickness(0, 0, 5, 0)
		};
		var txt = new TextBlock
		{
			Text = text,
			VerticalAlignment = VerticalAlignment.Center
		};
		stack.Children.Add(img);
		stack.Children.Add(txt);
		return stack;
	}

	private RelayCommand? _deleteCommand;
	public RelayCommand DeleteCommand => _deleteCommand ??= new RelayCommand(DeleteCommand_Execute);

	private void DeleteCommand_Execute(object? sender, object? parameter)
	{
		if (parameter is User user)
		{
			try
			{
				_repository.DeleteUser(user.Id!);
				NotifierService.Instance.UpdateStatus("User deleted.");
				LoadUsers();
				ClearForm();
			}
			catch (Exception ex)
			{
				NotifierService.Instance.UpdateStatus($"Error deleting user: {ex.Message}");
			}
		}
	}

}