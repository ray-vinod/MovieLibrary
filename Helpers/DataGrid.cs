using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MovieLibrary.Helpers;

public static class DataGrid
{
    // Find a button in a DataGrid row
    public static Button? FindButtonInRow(DataGridRow row, string buttonName)
    {
        if (row == null)
        {
            return null;
        }

        var stackPanel = FindVisualChild<StackPanel>(row);
        if (stackPanel == null)
        {
            return null;
        }

        foreach (var child in stackPanel.Children)
        {
            if (child is Button button && button.Name == buttonName)
            {
                return button;
            }
        }

        return null;
    }

    // Generic for find a visual child
    private static T? FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
    {
        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
        {
            DependencyObject child = VisualTreeHelper.GetChild(parent, i);
            if (child != null && child is T)
            {
                return (T)child;
            }

            T? childItem = FindVisualChild<T>(child!);
            if (childItem != null)
            {
                return childItem;
            }
        }

        return null;
    }
}