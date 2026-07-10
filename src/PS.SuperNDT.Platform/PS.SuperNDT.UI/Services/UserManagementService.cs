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

    public List<UserRoleModel> GetAll()
    {
        try
        {
            if (!File.Exists(_usersFile))
            {
                return new List<UserRoleModel>();
            }

            var json =
                File.ReadAllText(_usersFile);

            var users =
                JsonSerializer.Deserialize<List<UserRoleModel>>(json);

            return users ??
                   new List<UserRoleModel>();
        }
        catch
        {
            return new List<UserRoleModel>();
        }
    }

    public UserRoleModel? GetByUserName(
        string username)
    {
        return GetAll()
            .FirstOrDefault(x =>
                x.Username.Equals(
                    username,
                    StringComparison.OrdinalIgnoreCase));
    }

    public void AddOrUpdate(
        UserRoleModel user)
    {
        var users = GetAll();

        var existing =
            users.FirstOrDefault(x =>
                x.Username.Equals(
                    user.Username,
                    StringComparison.OrdinalIgnoreCase));

        if (existing == null)
        {
            users.Add(user);
        }
        else
        {
            existing.FullName = user.FullName;
            existing.Role = user.Role;
            existing.IsActive = user.IsActive;
        }

        Save(users);
    }

    public void Delete(
        string username)
    {
        var users = GetAll();

        users.RemoveAll(x =>
            x.Username.Equals(
                username,
                StringComparison.OrdinalIgnoreCase));

        Save(users);
    }

    private void Save(
        List<UserRoleModel> users)
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