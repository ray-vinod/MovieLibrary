using System.ComponentModel.DataAnnotations;

namespace MovieLibrary.Models;

public class BorrowRecord
{
	[Required]
	public string Id { get; set; } = "";

	[Required]
	public string? MovieId { get; set; } = "";

	[Required]
	public string? MovieTitle { get; set; } = "";

	[Required]
	public string UserId { get; set; } = "";

	[Required]
	public string? UserName { get; set; } = "";

	[Required]
	public string BorrowDate { get; set; } = DateTime.Now.ToShortDateString();

	public string? ReturnDate { get; set; }
}