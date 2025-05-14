using System.ComponentModel.DataAnnotations;

namespace MovieLibrary.Models;

public class BorrowRecord
{
    [Required]
    public string? Id { get; set; }

    [Required]
    public string? MovieId { get; set; }

    [Required]
    public string? UserId { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime BorrowDate { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime ReturnDate { get; set; }
}