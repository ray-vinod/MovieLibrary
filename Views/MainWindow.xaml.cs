using System.Windows;
using MovieLibrary.Services;
using MovieLibrary.Views.UserControls;

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

    private void UserManagement_Click(object sender, RoutedEventArgs e)
    {
        MainContent.Content = new UserManagement();
        StatusText.Text = $"App Status:- Movie Management";
    }

    private void BorrowHistory_Click(object sender, RoutedEventArgs e)
    {
        MainContent.Content = new BorrowReturn();
        StatusText.Text = $"App Status:- Borrow Return managemen";
    }


    private void HowToUse_Click(object sender, RoutedEventArgs e)
    {
        MainContent.Content = new HowToUse();
        StatusText.Text = $"App Status:- How to use";
    }

    private void About_Click(object sender, RoutedEventArgs e)
    {
        MainContent.Content = new About();
        StatusText.Text = $"App Status:- About";
    }
}