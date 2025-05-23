using System.Windows.Input;

namespace MovieLibrary.Helpers;

public class RelayCommand : ICommand
{
    private readonly Action<object, object> _execute;
    public RelayCommand(Action<object, object> execute) => _execute = execute;
    public bool CanExecute(object? parameter) => true;
    public void Execute(object? parameter) => _execute(null!, parameter ?? new object());
#pragma warning disable CS0067
    public event EventHandler? CanExecuteChanged;
#pragma warning restore CS0067
}