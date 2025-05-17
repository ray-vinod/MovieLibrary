using System.Windows.Controls;

namespace MovieLibrary.Services;

public class NotifierService
{
	private static NotifierService? _instance;
	public static NotifierService Instance => _instance ??= new NotifierService();

	public event Action<string?, string>? StatusUpdated;

	public void UpdateStatus(string? location = null, string message = "Ready")
	{
		StatusUpdated?.Invoke(location, message);
	}

	public void UpdateStatusLocation(HeaderedItemsControl control, string message = "Ready")
	{
		string location = control.Header?.ToString() ?? "Unknown";
		UpdateStatus(location, message);
	}
}