using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace MovieLibrary.Helpers;
public class WidthToVisibilityConverter : IValueConverter
{
	public double Threshold { get; set; } = 600;

	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value is double width)
		{
			return width < Threshold ? Visibility.Collapsed : Visibility.Visible;
		}
		return Visibility.Visible;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return Binding.DoNothing;
	}
}

