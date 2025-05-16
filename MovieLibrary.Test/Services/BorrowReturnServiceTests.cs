using MovieLibrary.Data;
using MovieLibrary.Models;
using MovieLibrary.Services;

namespace MovieLibrary.Test.Services;

public class BorrowReturnServiceTests
{
    public BorrowReturnServiceTests()
    {
        // Reset repository to avoid shared state between tests
        Repository.Instance.Reset();
    }

    [Fact]
    public void BorrowMovie_ShouldSucceed_WhenMovieAvailable()
    {
        // Arrange
        var repo = Repository.Instance;

        var movie = new Movie
        {
            Id = "M100",
            Title = "Test Movie",
            Director = "Test Director",
            Genre = "Test Genre",
            ReleaseYear = 2022,
            IsAvailable = true
        };
        repo.MovieRepo.AddMovie(movie);

        var user = new User
        {
            Id = "U100",
            Name = "Test User"
        };
        repo.UserRepo.AddUser(user);

        var service = new BorrowReturnService(repo.MovieRepo);

        // Act
        string result = service.BorrowMovie(movie.Id, user);

        // Assert
        Assert.Equal($"Movie '{movie.Title}' is now borrowed by {user.Name}.", result);
        Assert.False(movie.IsAvailable);
        Assert.Single(service.BorrowRecords);
        Assert.Equal(user.Id, service.BorrowRecords[0].UserId);
    }
}