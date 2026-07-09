using System;
using System.Collections.Generic;
using System.Linq;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class UserService
{
    private static readonly List<UserModel> _users = new();

    public UserService()
    {
        if (_users.Count == 0)
        {
            _users.Add(new UserModel
            {
                Username = "admin",
                Password = "admin",
                FullName = "System Administrator",
                Role = UserRole.Admin,
                IsActive = true
            });
        }
    }

    public List<UserModel> GetAll()
    {
        return _users
            .OrderBy(x => x.Username)
            .ToList();
    }

    public UserModel? GetById(Guid id)
    {
        return _users
            .FirstOrDefault(x => x.Id == id);
    }

    public UserModel? GetByUsername(string username)
    {
        return _users
            .FirstOrDefault(x =>
                string.Equals(
                    x.Username,
                    username,
                    StringComparison.OrdinalIgnoreCase));
    }

    public bool Create(UserModel user)
    {
        if (user == null)
            return false;

        if (_users.Any(x =>
                string.Equals(
                    x.Username,
                    user.Username,
                    StringComparison.OrdinalIgnoreCase)))
        {
            return false;
        }

        _users.Add(user);

        return true;
    }

    public UserModel? Login(
        string username,
        string password)
    {
        var user =
            _users.FirstOrDefault(x =>
                x.IsActive &&
                string.Equals(
                    x.Username,
                    username,
                    StringComparison.OrdinalIgnoreCase) &&
                x.Password == password);

        if (user != null)
        {
            user.LastLoginOn = DateTime.Now;
        }

        return user;
    }

    public void Delete(Guid id)
    {
        var user =
            _users.FirstOrDefault(x => x.Id == id);

        if (user != null)
        {
            _users.Remove(user);
        }
    }
}