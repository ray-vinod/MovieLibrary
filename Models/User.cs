using System.ComponentModel.DataAnnotations;

namespace MovieLibrary.Models;

public class User
{
    [Required]
    public string? Id { get; set; }

    [Required]
    public string? Name { get; set; }

    // User's borrowed movies
    public Dictionary<string, int> MovieViewCount { get; set; } = new();
}