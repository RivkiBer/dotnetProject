using UserNamespace.Models;
using UserService.interfaces;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using BakeryNamespace.Services;
namespace UserNamespace.Services;

/// <summary>
/// Service for User CRUD operations
/// Inherits generic CRUD logic from BaseGenericService
/// </summary>
public class UserService : BaseGenericService<User>, IUserService
{
    public UserService(IWebHostEnvironment webHost)
        : base(webHost, "Users.json")
    {
    }

    /// <summary>
    /// Create new user with encrypted password
    /// </summary>
    public override User Create(User newUser)
    {
        if (newUser == null)
            throw new ArgumentNullException(nameof(newUser));

        // Encrypt password using BCrypt
        newUser.Pass = BCrypt.Net.BCrypt.HashPassword(newUser.Pass);

        // Call base Create to handle ID assignment and saving
        return base.Create(newUser);
    }

    /// <summary>
    /// Get user by ID
    /// </summary>
    public User Get(int id) => FindById(id);

    public User GetRawByName(string name)
    {
    return list.FirstOrDefault(u => u.Name == name); 
       }

    /// <summary>
    /// Update user with validation
    /// Returns: 1 = not found, 2 = ID mismatch, 3 = success
    /// </summary>
    public int Update(int id, User newUser)
    {
        var user = FindById(id);
        if (user == null)
            return 1;

        if (user.Id != newUser.Id)
            return 2;

        var index = list.IndexOf(user);
        list[index] = newUser;
        SaveToFile();

        return 3;
    }
}
public static class UserServiceExtension
{
    public static void AddUserService(this IServiceCollection services)
    {
        services.AddSingleton<IUserService, UserService>();
    }
}

