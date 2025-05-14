using System.Windows;
using MovieLibrary.Services;
using MovieLibrary.Views.UserControlls;

namespace MovieLibrary.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        MainContent.Content = new Home();
        StatusText.Text = "Welcome to Movie Library";

        NotifierService.Instance.StatusUpdated += OnStatusUpdated;
    }

    private void OnStatusUpdated(string message)
    {
        Dispatcher.Invoke(() => StatusText.Text = $"App Status:- {message}");
    }

    protected override void OnClosed(EventArgs e)
    {
        NotifierService.Instance.StatusUpdated -= OnStatusUpdated;
        base.OnClosed(e);
    }

    private void Home_Click(object sender, RoutedEventArgs e)
    {
        MainContent.Content = new Home();
        StatusText.Text = $"App Status:- Welcome to Movie Library!";
    }

    private void MovieManagement_Click(object sender, RoutedEventArgs e)
    {
        MainContent.Content = new MovieManagement();
        StatusText.Text = $"App Status:- Movie Management";
    }


    private void UserAddNew_Click(object sender, RoutedEventArgs e)
    {

    }

    private void BorrowHistory_Click(object sender, RoutedEventArgs e)
    {
        MainContent.Content = new BorrowReturn();
        StatusText.Text = $"App Status:- Borrow Return managemen";
    }

    private void UserList_Click(object sender, RoutedEventArgs e)
    {

    }

    private void HowToUse_Click(object sender, RoutedEventArgs e)
    {

    }


}