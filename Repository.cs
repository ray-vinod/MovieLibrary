using MovieLibrary.Data;
using MovieLibrary.Helpers;
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
    public BorrowRecordRepository RecordRepo { get; set; }


    public Repository()
    {
        MovieRepo = new MovieRepository();
        UserRepo = new UserRepository();
        RecordRepo = new BorrowRecordRepository();

        var movies = MovieRepo.GetAllMovies();
        var movie1 = new Movie
        {
            Id = GenerateId.MovieId(movies),
            Title = "Inception",
            Director = "Christopher Nolan",
            Genre = "Sci-Fi",
            ReleaseYear = 2010
        };

        var movie2 = new Movie
        {
            Id = GenerateId.MovieId(movies),
            Title = "The Godfather",
            Director = "Francis Ford Coppola",
            Genre = "Crime",
            ReleaseYear = 1972
        };

        var movie3 = new Movie
        {
            Id = GenerateId.MovieId(movies),
            Title = "The Shawshank  Redemption",
            Director = "Frank Darabont",
            Genre = "Drama",
            ReleaseYear = 1994
        };

        MovieRepo.AddMovie(movie1);
        MovieRepo.AddMovie(movie2);
        MovieRepo.AddMovie(movie3);

        var users = UserRepo.GetAllUsers();
        var user1 = new User
        {
            Id = GenerateId.UserId(users),
            Name = "Bibek Subedi"
        };

        var user2 = new User
        {
            Id = GenerateId.UserId(users),
            Name = "Janu Adhikari"
        };

        UserRepo.AddUser(user1);
        UserRepo.AddUser(user2);
    }
}