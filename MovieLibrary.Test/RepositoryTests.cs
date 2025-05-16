namespace MovieLibrary.Test;

using MovieLibrary.Data;
using MovieLibrary.Models;
using Xunit;

public class RepositoryTests
{
    [Fact]
    public void Instance_ShouldReturnSameInstance()
    {
        // Arrange & Act
        var instance1 = Repository.Instance;
        var instance2 = Repository.Instance;

        // Assert
        Assert.Same(instance1, instance2);
    }

    [Fact]
    public void GenerateNewUserId_ShouldReturnNextId()
    {
        // Arrange
        var repo = new UserRepository();
        repo.AddUser(new User { Id = "U001", Name = "A" });
        repo.AddUser(new User { Id = "U002", Name = "B" });

        var repository = new Repository
        {
            UserRepo = repo
        };

        // Act
        var newId = repository.GenerateNewUserId();

        // Assert
        Assert.Equal("U003", newId);
    }

    [Fact]
    public void GenerateNewMovieId_ShouldReturnNextMovieId()
    {
        // Arrange
        var repository = new Repository();

        // Act
        var newId = repository.GenerateNewMovieId();

        // Assert
        Assert.Equal("M004", newId);
    }
}
