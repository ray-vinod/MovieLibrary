using MovieLibrary.Data;
using MovieLibrary.Models;

namespace MovieLibrary.Test.Data;

public class UserRepositoryTest
{
    [Fact]
    public void AddUser_ShouldAddUser_WhenUserIsValid()
    {
        // Arrange
        var repo = new UserRepository();
        var user = new User { Id = "U001", Name = "Test User" };

        // Act
        repo.AddUser(user);

        // Assert
        var retrieved = repo.GetUserById("U001");
        Assert.NotNull(retrieved);
        Assert.Equal("Test User", retrieved!.Name);
    }

    [Fact]
    public void AddUser_ShouldThrowException_WhenUserAlreadyExists()
    {
        // Arrange
        var repo = new UserRepository();
        var user = new User { Id = "U001", Name = "Test User" };
        repo.AddUser(user);

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => repo.AddUser(user));
        Assert.Contains("already exists", ex.Message);
    }

    [Fact]
    public void UpdateUser_ShouldReplaceOldUserWithNew()
    {
        // Arrange
        var repo = new UserRepository();
        var user = new User { Id = "U001", Name = "Test User" };
        repo.AddUser(user);
        var updatedUser = new User { Id = "U001", Name = "Updated Name" };

        // Act
        repo.UpdateUser(updatedUser);

        // Assert
        var retrieved = repo.GetUserById("U001");
        Assert.Equal("Updated Name", retrieved!.Name);
    }

    [Fact]
    public void DeleteUser_ShouldRemoveUser()
    {
        // Arrange
        var repo = new UserRepository();
        var user = new User { Id = "U001", Name = "Test User" };
        repo.AddUser(user);

        // Act
        repo.DeleteUser("U001");

        // Assert
        var retrieved = repo.GetUserById("U001");
        Assert.Null(retrieved);
    }

    [Fact]
    public void GetAllUsers_ShouldReturnAllUsers()
    {
        // Arrange
        var repo = new UserRepository();
        repo.AddUser(new User { Id = "U001", Name = "User1" });
        repo.AddUser(new User { Id = "U002", Name = "User2" });

        // Act
        var allUsers = repo.GetAllUsers().ToList();

        // Assert
        Assert.Equal(2, allUsers.Count);
    }
}
