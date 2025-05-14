using System.ComponentModel.DataAnnotations;

namespace MovieLibrary.Models;

public class Movie
{
    [Required]
    public string? Id { get; set; }

    [Required]
    public string? Title { get; set; }

    [Required]
    public string? Genre { get; set; }

    [Required]
    public int ReleaseYear { get; set; }

    [Required]
    public string? Director { get; set; }
    public bool IsAvailable { get; set; }

    // Waiting list of users
    public Queue<string> WaitingList { get; set; } = new();
}