namespace MovieLibrary.Services;

public class NotifierService
{
    private static NotifierService? _instance;
    public static NotifierService Instance => _instance ??= new NotifierService();

    public event Action<string>? StatusUpdated;

    public void UpdateStatus(string message)
    {
        StatusUpdated?.Invoke(message);
    }
}