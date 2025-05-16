using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MovieLibrary.Data;
using MovieLibrary.Helpers;
using MovieLibrary.Models;
using MovieLibrary.Services;

namespace MovieLibrary.Views.UserControls;

public partial class UserManagement : UserControl
{
    private readonly UserRepository _repository;
    private List<User> _allUsers = [];
    private User? _editingUser = null;

    private int _currentPage = 1;
    private int _pageSize = 10;
    private int _totalPages = 1;


    public UserManagement()
    {
        InitializeComponent();
        _repository = Repository.Instance.UserRepo;
        Loaded += (s, e) => LoadUsers();
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
    }


    private void LoadUsers()
    {
        _allUsers = _repository.GetAllUsers().ToList();
        _totalPages = (_allUsers.Count + _pageSize - 1) / _pageSize;

        DisplayPage(_currentPage);
    }


    private void AddOrSaveButton_Click(object sender, RoutedEventArgs e)
    {
        var name = NameBox.Input.Text.Trim();

        if (string.IsNullOrWhiteSpace(name))
        {
            NotifierService.Instance.UpdateStatus("ID and Name are required.");
            return;
        }

        if (_editingUser == null)
        {
            try
            {
                var user = new User
                {
                    Id = Repository.Instance.GenerateNewUserId(),
                    Name = name
                };

                _repository.AddUser(user);
                NotifierService.Instance.UpdateStatus("User added.");
            }
            catch (Exception ex)
            {
                NotifierService.Instance.UpdateStatus($"Error adding user: {ex.Message}");
            }
        }
        else
        {
            try
            {
                _editingUser.Name = name;
                _repository.UpdateUser(_editingUser);
                NotifierService.Instance.UpdateStatus("User updated.");
            }
            catch (Exception ex)
            {
                NotifierService.Instance.UpdateStatus($"Error updating user: {ex.Message}");
            }
        }

        LoadUsers();
        ClearForm();
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
            AddOrSaveButton.Content = CreateButtonContent("Save Changes", "/assets/add.png");
        }
    }

    private StackPanel CreateButtonContent(string text, string iconPath)
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