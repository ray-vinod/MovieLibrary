using System.ComponentModel.DataAnnotations;

namespace MovieLibrary.Models;

public class Movie
{
    public string Id { get; set; }

    [Required]
    public string Title { get; set; }

    [Required]
    public string Genre { get; set; }

    [Required]
    public int ReleaseYear { get; set; }

    [Required]
    public string Director { get; set; }
    public bool IsAvailable { get; set; }

    // Waiting list of users
    public Queue<BorrowRecord> WaitingList { get; set; } = new();

    public Movie(string id, string title, string genre, int releaseYear, string director)
    {
        Id = id;
        Title = title;
        Genre = genre;
        ReleaseYear = releaseYear;
        Director = director;
        IsAvailable = true;
    }

}