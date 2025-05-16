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

    [Fact]
    public void ReturnMovie_ShouldSucceed_WhenMovieIsBorrowed()
    {
        // Arrange
        Repository.Instance.Reset();
        var repo = Repository.Instance;

        var movie = new Movie
        {
            Id = "M101",
            Title = "Returnable Movie",
            IsAvailable = true
        };
        repo.MovieRepo.AddMovie(movie);

        var user = new User
        {
            Id = "U101",
            Name = "Returning User"
        };
        repo.UserRepo.AddUser(user);

        var service = new BorrowReturnService(repo.MovieRepo);
        service.BorrowMovie(movie.Id, user);

        // Act
        string result = service.ReturnMovie(movie.Id, user);

        // Assert
        Assert.Equal($"Movie '{movie.Title}' has been returned by {user.Name} and is now available for borrowing.", result);
        Assert.True(movie.IsAvailable);
        Assert.Empty(service.BorrowRecords);
    }

    [Fact]
    public void BorrowMovie_ShouldFail_WhenUserAlreadyBorrowedMovie()
    {
        // Arrange
        Repository.Instance.Reset();
        var repo = Repository.Instance;

        var movie = new Movie
        {
            Id = "M102",
            Title = "Duplicate Borrow",
            IsAvailable = true
        };
        repo.MovieRepo.AddMovie(movie);

        var user = new User
        {
            Id = "U102",
            Name = "Duplicate User"
        };
        repo.UserRepo.AddUser(user);

        var service = new BorrowReturnService(repo.MovieRepo);

        // Act
        service.BorrowMovie(movie.Id, user); // First borrow
        var result = service.BorrowMovie(movie.Id, user); // Second attempt

        // Assert
        Assert.Equal($"User '{user.Name}' has already borrowed this movie.", result);
    }

    [Fact]
    public void BorrowMovie_ShouldAddToWaitingList_WhenMovieIsUnavailable()
    {
        // Arrange
        Repository.Instance.Reset();
        var repo = Repository.Instance;

        var movie = new Movie
        {
            Id = "M103",
            Title = "Unavailable Movie",
            IsAvailable = false
        };
        repo.MovieRepo.AddMovie(movie);

        var user = new User
        {
            Id = "U103",
            Name = "Waiting User"
        };
        repo.UserRepo.AddUser(user);

        var service = new BorrowReturnService(repo.MovieRepo);

        // Act
        var result = service.BorrowMovie(movie.Id, user);

        // Assert
        Assert.Equal($"Movie '{movie.Title}' is currently unavailable. You have been added to the waiting list.", result);
        Assert.Contains(user.Id, movie.WaitingList);
    }

    [Fact]
    public void ReturnMovie_ShouldAutoIssueToNextWaitingUser()
    {
        // Arrange
        Repository.Instance.Reset();
        var repo = Repository.Instance;

        var movie = new Movie
        {
            Id = "M104",
            Title = "Queued Movie",
            IsAvailable = true
        };
        repo.MovieRepo.AddMovie(movie);

        var user1 = new User { Id = "U104", Name = "Borrower" };
        var user2 = new User { Id = "U105", Name = "Queued User" };

        repo.UserRepo.AddUser(user1);
        repo.UserRepo.AddUser(user2);

        var service = new BorrowReturnService(repo.MovieRepo);

        // User1 borrows it
        service.BorrowMovie(movie.Id, user1);
        // User2 joins queue
        service.BorrowMovie(movie.Id, user2);

        // Act
        var result = service.ReturnMovie(movie.Id, user1);

        // Assert
        Assert.Contains("automatically issued to next waiting user", result);
        Assert.Single(service.BorrowRecords);
        Assert.Equal(user2.Id, service.BorrowRecords[0].UserId);
    }

    [Fact]
    public void ReturnMovie_ShouldFail_WhenUserDidNotBorrow()
    {
        // Arrange
        Repository.Instance.Reset();
        var repo = Repository.Instance;

        var movie = new Movie
        {
            Id = "M105",
            Title = "Unborrowed Movie",
            IsAvailable = true
        };
        repo.MovieRepo.AddMovie(movie);

        var user = new User
        {
            Id = "U106",
            Name = "Non-Borrower"
        };
        repo.UserRepo.AddUser(user);

        var service = new BorrowReturnService(repo.MovieRepo);

        // Act
        var result = service.ReturnMovie(movie.Id, user);

        // Assert
        Assert.Equal("This user has not borrowed this movie.", result);
    }


}