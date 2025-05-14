using System.IO;
using System.Text.Json;
using MovieLibrary.Models;

namespace MovieLibrary.Services;

public class ImportExportService
{
    public static void ExportMovies(IEnumerable<Movie> movies, string filePath)
    {
        try
        {
            var json = JsonSerializer.Serialize(movies, new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            // Ensure the directory exists
            var directory = Path.GetDirectoryName(filePath);
            if (directory != null && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Write the JSON to the specified file
            using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            using var writer = new StreamWriter(fileStream);
            writer.Write(json);
            writer.Flush();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error exporting movies: {ex.Message}");
        }
    }

    public static IEnumerable<Movie> ImportMovies(string filePath)
    {
        try
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("File not found");
            }

            using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            using var reader = new StreamReader(fileStream);
            var json = reader.ReadToEnd();

            return JsonSerializer.Deserialize<IEnumerable<Movie>>(json) ?? Enumerable.Empty<Movie>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while importing movies: {ex.Message}");
            return Enumerable.Empty<Movie>();
        }
    }
}