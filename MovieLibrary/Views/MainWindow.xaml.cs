using System.Windows;
using System.Windows.Controls;

using MovieLibrary.Services;
using MovieLibrary.Views.UserControls;

namespace MovieLibrary.Views;

public partial class MainWindow : Window
{
	public MainWindow()
	{
		InitializeComponent();
		MainContent.Content = new Home();

		NotifierService.Instance.StatusUpdated += OnStatusUpdated;

		NotifierService.Instance.UpdateStatus("Main Window");
	}

	// Status display
	private void OnStatusUpdated(string? location, string message)
	{
		Dispatcher.Invoke(() =>
		{
			if (!string.IsNullOrWhiteSpace(location))
			{
				LocationText.Text = $"Status: {location}";
			}

			StatusText.Text = message;
		});
	}

	// event dispose
	protected override void OnClosed(EventArgs e)
	{
		NotifierService.Instance.StatusUpdated -= OnStatusUpdated;
		base.OnClosed(e);
	}

	public void ImportExport_Click(object sender, RoutedEventArgs e)
	{
		MainContent.Content = new ImportExport();
		NotifierService.Instance.UpdateStatusLocation((MenuItem)sender);
	}

	private void Home_Click(object sender, RoutedEventArgs e)
	{
		MainContent.Content = new Home();
		NotifierService.Instance.UpdateStatusLocation((MenuItem)sender);
	}

	private void MovieManagement_Click(object sender, RoutedEventArgs e)
	{
		MainContent.Content = new MovieManagement();
			NotifierService.Instance.UpdateStatusLocation((MenuItem)sender);
	}

	private void UserManagement_Click(object sender, RoutedEventArgs e)
	{
		MainContent.Content = new UserManagement();
		NotifierService.Instance.UpdateStatusLocation((MenuItem)sender);
	}

	private void BorrowHistory_Click(object sender, RoutedEventArgs e)
	{
		MainContent.Content = new BorrowReturn();
		NotifierService.Instance.UpdateStatusLocation((MenuItem)sender);
	}


	private void HowToUse_Click(object sender, RoutedEventArgs e)
	{
		MainContent.Content = new HowToUse();
		NotifierService.Instance.UpdateStatusLocation((MenuItem)sender);
	}

	private void About_Click(object sender, RoutedEventArgs e)
	{
		MainContent.Content = new About();
		NotifierService.Instance.UpdateStatusLocation((MenuItem)sender);
	}
}