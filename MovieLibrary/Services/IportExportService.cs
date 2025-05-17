using System.IO;
using System.Text.Json;

namespace MovieLibrary.Services;

public static class ImportExportService
{
	public static void ExportData<T>(IEnumerable<T> data, string filePath)
	{
		try
		{
			var json = JsonSerializer.Serialize(data, new JsonSerializerOptions
			{
				WriteIndented = true,
				PropertyNamingPolicy = JsonNamingPolicy.CamelCase
			});

			var directory = Path.GetDirectoryName(filePath);
			if (directory != null && !Directory.Exists(directory))
			{
				Directory.CreateDirectory(directory);
			}

			using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
			using var writer = new StreamWriter(fileStream);
			writer.Write(json);
			writer.Flush();
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error exporting data: {ex.Message}");
		}
	}

	public static IEnumerable<T> ImportData<T>(string filePath)
	{
		try
		{
			if (!File.Exists(filePath))
			{
				throw new FileNotFoundException("File not found", filePath);
			}

			using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
			using var reader = new StreamReader(fileStream);
			var json = reader.ReadToEnd();

			var options = new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true,
				PropertyNamingPolicy = JsonNamingPolicy.CamelCase
			};

			var data = JsonSerializer.Deserialize<IEnumerable<T>>(json, options);
			return data ?? Enumerable.Empty<T>();
		}
		catch (Exception ex)
		{
			Console.WriteLine($"An error occurred while importing data: {ex.Message}");
			return Enumerable.Empty<T>();
		}
	}
}

