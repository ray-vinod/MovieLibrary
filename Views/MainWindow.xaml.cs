using System.Windows;
using MovieLibrary.Views.UserControlls;

namespace MovieLibrary.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        MainContent.Content = new Home();
        StatusText.Text = "Welcome to Movie Library";
    }

    private void Home_Click(object sender, RoutedEventArgs e)
    {
        MainContent.Content = new Home();
        StatusText.Text = "Welcome to Movie Library!";
    }

    private void Movies_Click(object sender, RoutedEventArgs e)
    {

    }


    private void UserAddNew_Click(object sender, RoutedEventArgs e)
    {

    }

    private void BorrowHistory_Click(object sender, RoutedEventArgs e)
    {

    }

    private void UserList_Click(object sender, RoutedEventArgs e)
    {

    }

    private void HowToUse_Click(object sender, RoutedEventArgs e)
    {
        
    }


}