using MovieLibrary.Data;
using MovieLibrary.Models;

namespace MovieLibrary;

public class Repository
{
    private static Repository? _instance;

    public static Repository Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new Repository();
            }
            return _instance;
        }
    }

    public MovieRepository MovieRepo { get; set; }
    public UserRepository UserRepo { get; set; }
    public List<BorrowRecord> BorrowRecords { get; set; } = [];
    public BorrowRecordRepository RecordRepo { get; set; }

    public Repository()
    {
        MovieRepo = new MovieRepository();
        UserRepo = new UserRepository();
        RecordRepo = new BorrowRecordRepository();

        // Populate MovieRepo with test data
        var movie1 = new Movie
        {
            Id = "M001",
            Title = "Inception",
            Director = "Christopher Nolan",
            Genre = "Sci-Fi",
            ReleaseYear = 2010,
            IsAvailable = true
        };

        var movie2 = new Movie
        {
            Id = "M002",
            Title = "The Godfather",
            Director = "Francis Ford Coppola",
            Genre = "Crime",
            ReleaseYear = 1972,
            IsAvailable = true
        };

        var movie3 = new Movie
        {
            Id = "M003",
            Title = "The Shawshank Redemption",
            Director = "Frank Darabont",
            Genre = "Drama",
            ReleaseYear = 1994,
            IsAvailable = true
        };

        MovieRepo.AddMovie(movie1);
        MovieRepo.AddMovie(movie2);
        MovieRepo.AddMovie(movie3);

        var user1 = new User
        {
            Id = "U001",
            Name = "Bibek"
        };

        UserRepo.AddUser(user1);
    }


    public string GenerateNewMovieId()
    {
        var allMovies = MovieRepo.GetAllMovies();
        var maxId = allMovies
            .Select(m => int.TryParse(m.Id?.Substring(1), out var num) ? num : 0)
            .DefaultIfEmpty(0)
            .Max();

        return $"M{(maxId + 1):D3}";
    }

    public string GenerateNewUserId()
    {
        var allUsers = UserRepo.GetAllUsers();
        var maxId = allUsers
            .Select(u => int.TryParse(u.Id?.Substring(1), out var num) ? num : 0)
            .DefaultIfEmpty(0)
            .Max();

        return $"U{(maxId + 1):D3}";
    }

    public string GenerateNewRecordId()
    {
        var allRecords = RecordRepo.GetAllBorrowRecord();
        var maxId = allRecords
            .Select(r => int.TryParse(r.Id?.Substring(1), out var num) ? num : 0)
            .DefaultIfEmpty(0)
            .Max();

        return $"R{(maxId + 1):D3}";
    }

    public void Reset()
    {
        MovieRepo = new MovieRepository();
        UserRepo = new UserRepository();
        RecordRepo = new BorrowRecordRepository();
    }
}