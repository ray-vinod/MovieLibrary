using System.ComponentModel.DataAnnotations;

namespace MovieLibrary.Models;

public class BorrowRecord
{
    public string id { get; set; }

    [Required]
    public string MovieId { get; set; }

    [Required]
    public string UserId { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime BorrowDate { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime ReturnDate { get; set; }

    public BorrowRecord(string id, string movieId, string userId, DateTime borrowDate, DateTime returnDate)
    {
        this.id = id;
        MovieId = movieId;
        UserId = userId;
        BorrowDate = borrowDate;
        ReturnDate = returnDate;
    }
}