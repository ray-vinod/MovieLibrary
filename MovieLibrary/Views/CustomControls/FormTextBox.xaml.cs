using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace MovieLibrary.Views.CustomControls;
public partial class FormTextBox : UserControl, INotifyPropertyChanged
{
	public FormTextBox()
	{
		DataContext = this;
		InitializeComponent();
	}

	public event PropertyChangedEventHandler? PropertyChanged;
	private string _placeholder = "Your entry is here ...";


	public string Placeholder
	{
		get { return _placeholder; }
		set
		{
			_placeholder = value;
			OnPropertyChanged();
		}
	}

	private void OnPropertyChanged([CallerMemberName] string value = null!)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(value));
	}

	private void ClearButton_Click(object sender, RoutedEventArgs e)
	{
		Input.Clear();
		Input.Focus();
	}

	private void Input_TextChanged(object sender, TextChangedEventArgs e)
	{
		if (String.IsNullOrEmpty(Input.Text))
		{
			PlaceholderTextBlock.Visibility = Visibility.Visible;
			ClearButton.Visibility = Visibility.Hidden;
		}
		else
		{
			PlaceholderTextBlock.Visibility = Visibility.Hidden;
			ClearButton.Visibility = Visibility.Visible;
		}
	}
}
