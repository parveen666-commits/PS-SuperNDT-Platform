using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class UserManagementService
{
    private readonly string _usersFile =
        Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "users.json");

    public List<UserModel> GetAll()
    {
        try
        {
            if (!File.Exists(_usersFile))
            {
                return new List<UserModel>();
            }

            var json =
                File.ReadAllText(_usersFile);

            var users =
                JsonSerializer.Deserialize<List<UserModel>>(json);

            return users ??
                   new List<UserModel>();
        }
        catch
        {
            return new List<UserModel>();
        }
    }

    public UserModel? GetByUserName(
        string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            return null;

        return GetAll()
            .FirstOrDefault(x =>
                x.Username.Equals(
                    username,
                    StringComparison.OrdinalIgnoreCase));
    }

    public bool AddOrUpdate(
        UserModel user)
    {
        if (user == null ||
            string.IsNullOrWhiteSpace(user.Username))
        {
            return false;
        }

        var users = GetAll();

        var existing =
            users.FirstOrDefault(x =>
                x.Id == user.Id);

        if (existing == null)
        {
            var duplicateUsername =
                users.Any(x =>
                    x.Username.Equals(
                        user.Username,
                        StringComparison.OrdinalIgnoreCase));

            if (duplicateUsername)
                return false;

            users.Add(user);
        }
        else
        {
            var duplicateUsername =
                users.Any(x =>
                    x.Id != user.Id &&
                    x.Username.Equals(
                        user.Username,
                        StringComparison.OrdinalIgnoreCase));

            if (duplicateUsername)
                return false;

            existing.Username =
                user.Username;

            existing.FullName =
                user.FullName;

            existing.Password =
                user.Password;

            existing.Role =
                user.Role;

            existing.IsActive =
                user.IsActive;
        }

        Save(users);

        return true;
    }

    public bool Delete(
        string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            return false;

        var users = GetAll();

        var user =
            users.FirstOrDefault(x =>
                x.Username.Equals(
                    username,
                    StringComparison.OrdinalIgnoreCase));

        if (user == null)
            return false;

        if (string.Equals(
                user.Username,
                "admin",
                StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        users.Remove(user);

        Save(users);

        return true;
    }

    private void Save(
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
}