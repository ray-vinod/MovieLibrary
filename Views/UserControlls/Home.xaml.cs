using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MovieLibrary.Views.UserControlls;

public partial class Home : UserControl
{
    public Home()
    {
        InitializeComponent();
        PictureBox1.MouseWheel += PictureBox1_MouseWheel;
    }

    private void PictureBox1_MouseWheel(object sender, MouseWheelEventArgs e)
    {
        var zoomFactor = e.Delta > 0 ? 0.1 : -0.1;

        var transform = (ScaleTransform)PictureBox1.RenderTransform;
        transform.ScaleX += zoomFactor;
        transform.ScaleY += zoomFactor;

        // Prevent scaling below a certain threshold
        if (transform.ScaleX < 0.1 || transform.ScaleY < 0.1)
        {
            transform.ScaleX = 0.1;
            transform.ScaleY = 0.1;
        }
    }
}