using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Microsoft.Win32;
using MovieLibrary.Data;
using MovieLibrary.Services;

namespace MovieLibrary.Views.UserControls;

public partial class ImportExport : UserControl
{
    private MovieRepository _movieRepository;
    public ImportExport()
    {
        InitializeComponent();

        _movieRepository = Repository.Instance.MovieRepo;
    }

    public void ImportMovieButton_Click(object sender, RoutedEventArgs e)
    {
        OpenFileDialog openFileDialog = new OpenFileDialog { Filter = "JSON File|*.json" };
        if (openFileDialog.ShowDialog() == true)
        {
            try
            {
                var movies = ImportExportService.ImportMovies(openFileDialog.FileName);

                foreach (var movie in movies)
                {
                    try
                    {
                        var IsAvailable = _movieRepository.GetMovieById(movie.Id!);
                        if (IsAvailable == null)
                        {
                            _movieRepository.AddMovie(movie);
                        }
                    }
                    catch (Exception)
                    {
                        NotifierService.Instance.UpdateStatus("Movies import failed!");

                    }
                }

                NotifierService.Instance.UpdateStatus("Movies imported successfully.");
            }
            catch (Exception)
            {
                NotifierService.Instance.UpdateStatus("Import failled!");
            }
        }
    }

    public void ExportMovieButton_Click(object sender, RoutedEventArgs e)
    {
        SaveFileDialog fileDialog = new SaveFileDialog { Filter = "JSON File|*.json" };

        if (fileDialog.ShowDialog() == true)
        {
            try
            {
                ImportExportService.ExportMovies(_movieRepository.GetAllMovies(), fileDialog.FileName);
                NotifierService.Instance.UpdateStatus($"Movies exported successfully.");
            }
            catch (Exception)
            {
                NotifierService.Instance.UpdateStatus($"Export Failed!");
            }
        }
    }
}