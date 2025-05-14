using MovieLibrary.Models;

namespace MovieLibrary.Helpers;

public static class GenerateId
{
    public static string BorrowRecordId(IEnumerable<BorrowRecord> records)
    {
        return $"B{(records.Count() + 1).ToString("D3")}";
    }

    public static string MovieId(IEnumerable<Movie> movies)
    {
        return $"M{(movies.Count() + 1).ToString("D3")}";
    }

    public static string UserId(IEnumerable<User> users)
    {
        return $"U{(users.Count() + 1).ToString("D3")}";
    }
}