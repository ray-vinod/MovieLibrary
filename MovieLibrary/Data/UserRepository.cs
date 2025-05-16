using System.Collections;
using MovieLibrary.Models;

namespace MovieLibrary.Data;

public class UserRepository
{
    private readonly Hashtable _userLookup = new();
    private readonly LinkedList<User> _users = new();

    public void AddUser(User user)
    {
        if (_userLookup.ContainsKey(user.Id!))
        {
            throw new ArgumentException($"User with ID {user.Id} already exists.");
        }

        if (string.IsNullOrWhiteSpace(user.Id) || string.IsNullOrWhiteSpace(user.Name))
        {
            throw new ArgumentException("User ID and Name cannot be empty.");
        }

        _userLookup.Add(user.Id, user);
        _users.AddLast(user);
    }

    public User? GetUserById(string id)
    {
        return _userLookup.ContainsKey(id) ? _userLookup[id] as User : null;
    }
    public IEnumerable<User> GetAllUsers()
    {
        return _users;
    }

    public void UpdateUser(User user)
    {
        if (_userLookup.ContainsKey(user.Id!))
        {
            _userLookup[user.Id!] = user;
            var existingUser = _users.FirstOrDefault(u => u.Id == user.Id);
            if (existingUser != null)
            {
                _users.Remove(existingUser);
                _users.AddLast(user);
            }
        }
    }

    public void DeleteUser(string id)
    {
        if (_userLookup.ContainsKey(id))
        {
            if (_userLookup[id] is User user)
            {
                _users.Remove(user);
                _userLookup.Remove(id);
            }
        }
    }
}