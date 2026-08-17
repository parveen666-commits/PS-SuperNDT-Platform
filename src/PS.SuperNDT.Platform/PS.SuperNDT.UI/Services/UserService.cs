using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class UserService
{
    private readonly string _usersFile =
        Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "users.json");

    public UserService()
    {
        EnsureDefaultAdmin();
    }

    public List<UserModel> GetAll()
    {
        return LoadUsers()
            .OrderBy(x => x.Username)
            .ToList();
    }

    public UserModel? GetById(Guid id)
    {
        return LoadUsers()
            .FirstOrDefault(x => x.Id == id);
    }

    public UserModel? GetByUsername(
        string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            return null;

        return LoadUsers()
            .FirstOrDefault(x =>
                string.Equals(
                    x.Username,
                    username,
                    StringComparison.OrdinalIgnoreCase));
    }

    public bool Create(UserModel user)
    {
        if (user == null ||
            string.IsNullOrWhiteSpace(user.Username))
        {
            return false;
        }

        var users = LoadUsers();

        if (users.Any(x =>
                string.Equals(
                    x.Username,
                    user.Username,
                    StringComparison.OrdinalIgnoreCase)))
        {
            return false;
        }

        users.Add(user);

        SaveUsers(users);

        return true;
    }

    public UserModel? Login(
        string username,
        string password)
    {
        var users = LoadUsers();

        var user =
            users.FirstOrDefault(x =>
                x.IsActive &&
                string.Equals(
                    x.Username,
                    username,
                    StringComparison.OrdinalIgnoreCase) &&
                x.Password == password);

        if (user == null)
            return null;

        user.LastLoginOn = DateTime.Now;

        SaveUsers(users);

        return user;
    }

    public void Delete(Guid id)
    {
        var users = LoadUsers();

        var user =
            users.FirstOrDefault(x =>
                x.Id == id);

        if (user == null)
            return;

        if (string.Equals(
                user.Username,
                "admin",
                StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        users.Remove(user);

        SaveUsers(users);
    }

    private List<UserModel> LoadUsers()
    {
        try
        {
            if (!File.Exists(_usersFile))
            {
                return new List<UserModel>();
            }

            var json =
                File.ReadAllText(_usersFile);

            return JsonSerializer.Deserialize<List<UserModel>>(json)
                   ?? new List<UserModel>();
        }
        catch
        {
            return new List<UserModel>();
        }
    }

    private void SaveUsers(
        List<UserModel> users)
    {
        var json =
            JsonSerializer.Serialize(
                users,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                });

        File.WriteAllText(
            _usersFile,
            json);
    }

    private void EnsureDefaultAdmin()
    {
        var users = LoadUsers();

        var admin =
            users.FirstOrDefault(x =>
                string.Equals(
                    x.Username,
                    "admin",
                    StringComparison.OrdinalIgnoreCase));

        if (admin == null)
        {
            users.Add(
                new UserModel
                {
                    Username = "admin",
                    Password = "admin",
                    FullName = "System Administrator",
                    Role = UserRole.Admin,
                    IsActive = true
                });

            SaveUsers(users);
        }
    }
}